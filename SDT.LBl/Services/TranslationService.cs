using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SDT.Data.Enums;
using SDT.Data.Models;
using SDT.LBl.IServices;
using SDT.Repositories;
using System.Collections.Immutable;
using System.Diagnostics;
using Tiktoken;

namespace SDT.LBl
{
    public class TranslationService : ITranslationService
    {
        private readonly IOpenAIService _openAIService;

        private readonly IDocumentRepository _documentRepository;
        private readonly ITranslationRepository _translationRepository;
        private readonly ITranslationTaskRepository _translationTaskRepository;

        private readonly string _documentsDirectory;

        private static readonly ImmutableDictionary<string, string> Prompts = new Dictionary<string, string>
        {
            {
                "en", "Translate into English, preserving its literary style, tone, and nuances. Pay close attention to metaphors, cultural references, and emotional depth. Ensure names and linguistic details are accurate for the language. Do not add anything of your own. The main character is male."
            },
            {
                "ru", "Переведи на русский язык, сохраняя литературный стиль, тон и нюансы. Обрати внимание на метафоры, культурные отсылки и эмоциональную глубину. Убедись, что имена и языковые детали соответствуют языку. Не добавляй ничего от себя. Главный герой - мужского пола."
            },
            {
                "es", "Traduce al español, manteniendo su estilo literario, tono y matices. Presta especial atención a las metáforas, referencias culturales y profundidad emocional. Asegúrate de que los nombres y los detalles lingüísticos sean precisos en el idioma. No añadas nada de tu parte. El protagonista es masculino."
            },
            {
                "fr", "Traduisez en français, en conservant son style littéraire, son ton et ses nuances. Accordez une attention particulière aux métaphores, aux références culturelles et à la profondeur émotionnelle. Assurez-vous que les noms et les détails linguistiques sont précis pour la langue. N'ajoutez rien de vous-même. Le personnage principal est un homme."
            },
            {
                "ar", "ترجم إلى اللغة العربية مع الحفاظ على أسلوبه الأدبي ونبرته وجوانبه الفنية. انتبه إلى الاستعارات والإشارات الثقافية والعمق العاطفي. تأكد من أن الأسماء والتفاصيل اللغوية دقيقة في اللغة. لا تضف أي شيء من عندك. الشخصية الرئيسية ذكورية."
            }
        }.ToImmutableDictionary();

        public TranslationService(IOpenAIService openAIService, 
            ITranslationRepository translationRepository,
            IDocumentRepository documentRepository,
            ITranslationTaskRepository translationTaskRepository)
        {
            _openAIService = openAIService;
            _translationRepository = translationRepository;
            _translationTaskRepository = translationTaskRepository;

            // Получаем путь к папке documents в корне приложения
            _documentsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            // Создаем папку, если она не существует
            if (!Directory.Exists(_documentsDirectory))
            {
                Directory.CreateDirectory(_documentsDirectory);
            }

            _documentRepository = documentRepository;
        }

        public async Task<TranslationOptionsDto> GetTranslationOptions()
        {
            var companies = await _translationRepository.GetAICompany();
            var models = await _translationRepository.GetAILanguageModels();
            var languages = await _translationRepository.GetLanguages();

            return new TranslationOptionsDto
            {
                Companies = companies.Select(c => new AICompanyDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList(),
                Models = models.Select(m => new AILanguageModelDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AICompanyId = m.AICompanyId,
                    InputTokenPrice = m.InputTokenPrice,
                    OutputTokenPrice = m.OutputTokenPrice,
                    CachedTokenPrice = m.CachedTokenPrice
                }).ToList(),
                Languages = languages.Select(l => new LanguageDto
                {
                    Id = l.Id,
                    Code = l.Code,
                    Name = l.Name
                }).ToList()
            };
        }

        public async Task<List<UserDocumentDto>> GetUserDocuments(long userId)
        {
            var documents = await _documentRepository.GetDocumentByUserId(userId);

            return documents
                .Where(x => x.DocumentParentId == null)
                .Select(d => new UserDocumentDto
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToList();
        }

        public async Task TranslateDocument(TranslateDocumentDto documentDto)
        {
            var languageModel = await _translationRepository.GetAILanguageModelById(documentDto.LanguageModelId);
            var encoder = ModelToEncoder.For(languageModel.Name); // or explicitly using new Encoder(new O200KBase())

            var languages = await _translationRepository.GetLanguages();

            // Формируем путь для сохранения файла
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // Формат: ГГГГММДД_ччммсс
            var uniqueFileName = $"{timestamp} {documentDto.FileName}"; // Добавляем временной штамп к имени файла
            var filePath = Path.Combine(_documentsDirectory, uniqueFileName);

            // Сохраняем файл на диск
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await documentDto.FileStream.CopyToAsync(fileStream);
            }

            var document = new UserDocument
            {
                UserId = 1, // TODO: Получить ID пользователя из контекста
                Name = uniqueFileName,
                MimeType = documentDto.MimeType,
                FilePath = filePath
            };

            // Извлекаем абзацы из документа
            var paragraphDtos = ExtractParagraphsFromDocx(filePath);

            // Создаем коллекцию параграфов
            var paragraphs = paragraphDtos
                .Select(p => new OriginalParagraph
                {
                    Text = p.Text,
                    CharacterCount = p.CharacterCount,
                    EstimatedTokenCount = encoder.CountTokens(p.Text) * 2,
                    EstimatedPrice = encoder.CountTokens(p.Text) * languageModel.InputTokenPrice 
                                        + encoder.CountTokens(p.Text) * languageModel.OutputTokenPrice * 1.2f,
                    Document = document // Связываем параграфы с документом
                })
                .ToList();

            // Добавляем документ и его параграфы в базу данных
            document.Paragraphs = paragraphs; // Связываем параграфы с документом

            await _documentRepository.AddDocument(document);

            var estimatedTokenCount = paragraphs.Sum(p => p.EstimatedTokenCount);
            var estimatedPrice = paragraphs.Sum(p => p.EstimatedPrice);

            await _translationTaskRepository.AddedTanslationTasks(documentDto.TargetLanguageIds.Select(languageId => new TranslationTask
            {
                LanguageId = languageId,
                DocumentId = document.Id,
                Status = TranslationTaskStatus.New,
                ApiKey = documentDto.ApiKey,
                AILanguageModelId = documentDto.LanguageModelId,
                Prompt = Prompts.TryGetValue(languages.Find(l => l.Id == languageId)?.Code ?? "en", out var prompt)
                                                ? prompt
                                                : string.Empty,
                EstimatedTokenCount = estimatedTokenCount,
                EstimatedPrice = estimatedPrice,
                Progress = 0
            }).ToList());
        }

        private List<ParagraphDto> ExtractParagraphsFromDocx(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                // Проверяем MainDocumentPart, Document и Body на null
                if (wordDoc.MainDocumentPart?.Document?.Body == null)
                {
                    return new List<ParagraphDto>(); // Возвращаем пустой список, если структура документа невалидна
                }

                var paragraphs = wordDoc.MainDocumentPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();

                return paragraphs
                    .Where(p => !string.IsNullOrWhiteSpace(p.InnerText)) // Проверяем InnerText на null или пустоту
                    .Select((p, index) => new ParagraphDto
                    {
                        Id = index + 1,
                        Text = p.InnerText.Trim().Replace("\u00A0", " "), // Удаляем неразрывные пробелы
                        CharacterCount = p.InnerText.Trim().Replace("\u00A0", " ").Length
                    })
                    .ToList();
            }
        }

        public async Task<TranslationTasksDto> GetTranslationTasks(long documentId)
        {
            var document = await _documentRepository.GetDocumentById(documentId, query => query
                .Include(x => x.TranslationTasks)
                    .ThenInclude(x=> x.LanguageCode));
            
            var translationTasks = document.TranslationTasks;

            var documentInfo = new DocumentInfoDto
            {
                UploadDateTime = document.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Name = document.Name,
                TokenCount = translationTasks.Sum(t => t.FinalTokenCount ?? t.EstimatedTokenCount ?? 0),
                Price = translationTasks.Sum(t => t.FinalPrice ?? t.EstimatedPrice ?? 0),
                Progress = (float)Math.Round(translationTasks.Sum(t => t.Progress) / translationTasks.Count,2)
            };

            var translationTaskDtos = translationTasks.Select(t => new TranslationTaskDto
            {
                Id = t.Id,
                Status = t.Status.ToString(),
                Prompt = t.Prompt,
                TargetLanguage = t.LanguageCode?.Name ?? string.Empty,
                TokenCount = t.FinalTokenCount ?? t.EstimatedTokenCount ?? 0,
                Price = t.FinalPrice ?? t.EstimatedPrice ?? 0,
                Progress = t.Progress,
                TranslatedDocumentId = t.TranslatedDocumentId
            })
            .OrderBy(x => x.Id)
            .ToList();

            return new TranslationTasksDto
            {
                DocumentInfo = documentInfo,
                TranslationTasks = translationTaskDtos
            };
        }

        public async Task RunTranslation(long taskId)
        {
            var task = await _translationTaskRepository.GetTranslationTaskById(taskId, query => query
                .Include(x => x.AILanguageModel)
                .Include(x => x.ParagraphTranslations)
                .Include(x => x.Document)
                    .ThenInclude(x => x.Paragraphs)
                        .ThenInclude(x => x.ParagraphTranslations));

            var paragraphs = task.Document?.Paragraphs
                                .Where(x => !x.ParagraphTranslations.Any(pt => pt.TranslationTaskId == task.Id))
                                .OrderBy(x => x.Id)
                                .ToList() ?? [];

            if (paragraphs.Count > 0) 
            {
                task.Status = TranslationTaskStatus.InProgress;
                await _translationTaskRepository.UpdateTanslationTask(task);

                try 
                {
                    var model = task.AILanguageModel;

                    if (model == null)
                    {
                        throw new InvalidOperationException("Language model not found");
                    }

                    foreach (var paragraph in paragraphs)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        var chatCompletion = await _openAIService.GetAnswer(task.ApiKey, model.Name, task.Prompt, paragraph.Text);
                        stopwatch.Stop();
                        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                        var translation = new ParagraphTranslation
                        {
                            ParagraphId = paragraph.Id,
                            TranslationTaskId = task.Id,
                            TranslatedText = chatCompletion.Content[0].Text,
                            OutputTokenCount = chatCompletion.Usage.OutputTokenCount,
                            InputTokenCount = chatCompletion.Usage.InputTokenCount,
                            FinalPrice = (chatCompletion.Usage.OutputTokenCount * model.OutputTokenPrice +
                                         chatCompletion.Usage.InputTokenCount * model.InputTokenPrice) / 1000000,
                            ProcessingTime = (int)elapsedSeconds
                        };

                        task.ParagraphTranslations.Add(translation);
                        task.FinalTokenCount = task.ParagraphTranslations.Sum(x => x.OutputTokenCount) +
                                               task.ParagraphTranslations.Sum(x => x.InputTokenCount);
                        task.FinalPrice = task.ParagraphTranslations.Sum(x => x.FinalPrice);
                        task.Progress = (float)Math.Round((double)task.ParagraphTranslations.Count / paragraphs.Count * 100, 2);
                        task.ProcessingTime = task.ParagraphTranslations.Sum(x => x.ProcessingTime);

                        if (task.Progress >= 100)
                        {
                            task.Status = TranslationTaskStatus.Completed;
                        }
                        else
                        {
                            task.Status = TranslationTaskStatus.InProgress;
                        }

                        await _translationTaskRepository.UpdateTanslationTask(task);
                    }
                    await GenerateTranslatedDocument(taskId);
                }
                catch (Exception ex)
                {
                    task.Status = TranslationTaskStatus.Error;
                    task.ErrorDescription = ex.Message;
                    await _translationTaskRepository.UpdateTanslationTask(task);
                    return;
                }                
            }
        }

        private async Task GenerateTranslatedDocument(long taskId)
        {
            var task = await _translationTaskRepository.GetTranslationTaskById(taskId, query => query
                .Include(x => x.LanguageCode)
                .Include(x => x.TranslatedDocument)
                .Include(x => x.ParagraphTranslations)
                .Include(x => x.Document)
                    .ThenInclude(x => x.Paragraphs));

            if (task == null)
            {
                throw new InvalidOperationException("Translation task not found.");
            }

            if (task.ParagraphTranslations.Count == 0)
            {
                throw new InvalidOperationException("No translations found for the task.");
            }

            // Формируем уникальное имя файла
            var translatedFileName = $"({task.LanguageCode?.Name.ToLower()}) {task.Document?.Name}";
            var filePath = Path.Combine(_documentsDirectory, translatedFileName);

            // Создаём документ DOCX
            using (var wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document { Body = new Body() };
                var body = mainPart.Document.Body;

                // Добавляем переведённые параграфы с форматированием
                foreach (var translation in task.ParagraphTranslations.OrderBy(pt => pt.ParagraphId))
                {
                    // Создаем текст
                    var text = new Text(translation.TranslatedText) { Space = SpaceProcessingModeValues.Preserve };

                    // Создаем элемент Run (контейнер для текста)
                    var run = new Run(text);

                    // Добавляем свойства текста (размер шрифта)
                    var runProperties = new RunProperties();
                    runProperties.AppendChild(new FontSize { Val = "24" }); // Размер шрифта 12 (24 half-points)
                    run.PrependChild(runProperties);

                    // Создаем параграф с форматированием
                    var paragraph = new Paragraph(
                        new ParagraphProperties(
                            new Justification { Val = JustificationValues.Both }, // Выравнивание по ширине
                            new SpacingBetweenLines { After = "200" }, // Пустая строка после абзаца
                            new Indentation { FirstLine = "720" } // Красная строка
                        ),
                        run
                    );

                    body.Append(paragraph);
                }

                mainPart.Document.Save();
            }

            // Сохраняем информацию о переведённом документе в базе данных
            var translatedDocument = new UserDocument
            {
                Name = translatedFileName,
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                FilePath = filePath,
                DocumentParentId = task.Document.Id, // Связываем с исходным документом
                UserId = task.Document.UserId, // Устанавливаем владельца документа
                LanguageId = task.LanguageId,
            };

            await _documentRepository.AddDocument(translatedDocument);

            // Обновляем задачу с ссылкой на переведённый документ
            task.TranslatedDocumentId = translatedDocument.Id;
            await _translationTaskRepository.UpdateTanslationTask(task);
        }
    }

    public class TranslateDocumentDto
    {
        public required Stream FileStream { get; set; }
        public required string FileName { get; set; }
        public required string MimeType { get; set; } // MIME-тип
        public List<int> TargetLanguageIds { get; set; } = new();
        public required int LanguageModelId { get; set; }
        public required string ApiKey { get; set; }
    }

    public class ParagraphDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CharacterCount { get; set; }
    }

    public class TranslationOptionsDto
    {
        public List<AICompanyDto> Companies { get; set; }
        public List<AILanguageModelDto> Models { get; set; }
        public List<LanguageDto> Languages { get; set; }
    }

    public class AICompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AILanguageModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AICompanyId { get; set; }
        public float InputTokenPrice { get; set; }
        public float OutputTokenPrice { get; set; }
        public float CachedTokenPrice { get; set; }
    }

    public class LanguageDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class UserDocumentDto
    {
        public long Id { get; set; }
        public required string Name { get; set; }
    }

    /// <summary>
    /// DTO для передачи данных о заданиях на перевод
    /// </summary>
    public class TranslationTasksDto
    {
        public required DocumentInfoDto DocumentInfo { get; set; }
        public List<TranslationTaskDto> TranslationTasks { get; set; } = [];
    }

    public class DocumentInfoDto
    {
        public required string UploadDateTime { get; set; }
        public required string Name { get; set; }
        public int TokenCount { get; set; }
        public float Price { get; set; }
        public float Progress { get; set; }
    }

    public class TranslationTaskDto
    {
        public required long Id { get; set; }
        public required string Prompt { get; set; }
        public required string TargetLanguage { get; set; }
        public long? TranslatedDocumentId { get; set; }
        public int TokenCount { get; set; }
        public float Price { get; set; }
        public float Progress { get; set; }
        public required string Status { get; set; }
    }
}