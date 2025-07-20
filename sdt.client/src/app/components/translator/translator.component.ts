import { Component, OnDestroy, OnInit } from '@angular/core';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { TranslateDocumentRequest, TranslationTaskDto, TranslationTasksDto, UserDocumentDto } from '../../models/translation.model';
import { TranslationService } from '../../services/translation.service';
import { catchError, interval, of, Subscription, switchMap, takeWhile, tap } from 'rxjs';
import { AICompanyDto, AILanguageModelDto, LanguageDto } from '../../models/translation-options.model';
import { NzIconService } from 'ng-zorro-antd/icon';
import { DownloadOutline, InboxOutline, PlayCircleOutline } from '@ant-design/icons-angular/icons';
import { NzDescriptionsSize } from 'ng-zorro-antd/descriptions';
import { FileService } from '../../services/file.service';

@Component({
  selector: 'app-translator',
  templateUrl: './translator.component.html',
  styleUrl: './translator.component.scss'
})
export class TranslatorComponent implements OnInit, OnDestroy {
  private readonly subscriptions = new Subscription();

  public aiCompanies!: AICompanyDto[];
  public selectedAiCompanyId?: number | null;

  public aiLanguageModels!: AILanguageModelDto[];  
  public languages!: LanguageDto[];

  public translationRequest: TranslateDocumentRequest;

  public userDocuments?: UserDocumentDto[];
  public selectedUserDocumentId?: number | null;
  public translationTasks?: TranslationTasksDto;

  public size: NzDescriptionsSize = 'small';

  private pollingSubscription: Subscription | null = null;
  public runTranslationCount: number = 0;
  public updateTranslationTasksInterval: number = 2000;

  constructor(private readonly translationService: TranslationService,
    private readonly fileService: FileService,
    private readonly iconService: NzIconService
  ) {
    this.iconService.addIcon(InboxOutline, PlayCircleOutline, DownloadOutline);

    this.translationRequest = {
      selectedFile: undefined, // или файл, если он уже выбран
      targetLanguageIds: [], // начальное значение пустого массива
    } as TranslateDocumentRequest;
  }

  ngOnInit(){
    const subscription = this.translationService.getTranslationOptions().subscribe({
      next: (data) => {
      // Устанавливаем компании
      this.aiCompanies = data.companies;
      if (this.aiCompanies.length > 0) {
        this.selectedAiCompanyId = this.aiCompanies[0].id; // Выбираем первую компанию
      }

      // Фильтруем модели по первой компании
      this.aiLanguageModels = data.models.filter(
        (model) => model.aiCompanyId === this.selectedAiCompanyId
      );

      if (this.aiLanguageModels.length > 0) {
        this.translationRequest.languageModelId = this.aiLanguageModels[0].id; // Устанавливаем первую модель
      }

      // Устанавливаем языки
      this.languages = data.languages;
      },
      error: (error) => {
        console.error('Translation failed', error);
      },
      complete: () => {
        console.log('Translation request completed');
      }
    });

    this.subscriptions.add(subscription);
    this.getUserDocuments();
  }
  
  getStatusText(status: string): string {
    const statusMap: { [key: string]: string } = {
      'New': 'Новый',
      'InProgress': 'В процессе',
      'Completed': 'Завершен',
      'Error': 'Ошибка'
    };
    return statusMap[status] || status;
  }

  handleDocumentUpload(event: { file: NzUploadFile, fileList: NzUploadFile[] }): void {
    this.translationRequest.selectedFile = event.file.originFileObj;
  }

  onTranslate() {
    if (!this.isTranslationRequestValid()) {
      console.error('Translation request is invalid. Please fill in all required fields.');
      return;
    }
  
    const formData: FormData = new FormData();
  
    if (this.translationRequest.selectedFile) {
      formData.append('document', this.translationRequest.selectedFile, this.translationRequest.selectedFile.name);
    }
  
    this.translationRequest.targetLanguageIds.forEach(id => {
      formData.append('targetLanguageIds', id.toString());
    });
  
    formData.append('languageModelId', this.translationRequest.languageModelId!.toString());
    formData.append('apiKey', this.translationRequest.apiKey!);
  
    this.subscriptions.add(
      this.translationService.translateDocument(formData).subscribe({
        next: (data) => {
          console.log('Translation completed', data);
          this.getUserDocuments();
        },
        error: (error) => {
          console.error('Error during translation', error);
        },
        complete: () => {
          console.log('Translation request completed');
        }
      })
    );
  }  

  isTranslationRequestValid(): boolean {
    // Проверяем, что файл выбран
    if (!this.translationRequest.selectedFile) {
      console.warn('No file selected');
      return false;
    }
  
    // Проверяем, что список целевых языков не пуст
    if (!this.translationRequest.targetLanguageIds || this.translationRequest.targetLanguageIds.length === 0) {
      console.warn('No target languages selected');
      return false;
    }
  
    // Проверяем, что модель языка выбрана
    if (!this.translationRequest.languageModelId) {
      console.warn('No language model selected');
      return false;
    }
  
    // Проверяем, что API ключ указан
    if (!this.translationRequest.apiKey || this.translationRequest.apiKey.trim() === '') {
      console.warn('API key is missing');
      return false;
    }
  
    return true;
  }

  getUserDocuments() {
    const subscription = this.translationService.getUserDocuments().subscribe({
      next: (data) => {
          // Sort the documents by Id in descending order
          this.userDocuments = data.sort((a: any, b: any) => b.id - a.id);

          // Select the first document's Id
          this.selectedUserDocumentId = this.userDocuments.length > 0 ? this.userDocuments[0].id : null;
          this.getTranslationTasks()
      },
      error: (error) => {
        console.error('Translation failed', error);
      },
      complete: () => {
        console.log('Translation request completed');
      }
    });

    this.subscriptions.add(subscription)
  }  

  getTranslationTasks(): void {
    if (!this.selectedUserDocumentId) return;
  
    const subscription = this.translationService.getTranslationTasks(this.selectedUserDocumentId).subscribe({
      next: (data) => {
        this.translationTasks = data;
        this.runTranslationCount = this.translationTasks?.translationTasks.filter(task => task.status === 'InProgress').length || 0;
  
        // Автоматическое управление пулингом
        if (this.runTranslationCount > 0 && !this.pollingSubscription) {
          this.startTranslationTasksPolling();
        } else if (this.runTranslationCount === 0) {
          this.stopTranslationTasksPolling();
        }
      },
      error: (error) => {
        console.error('Error fetching translation tasks:', error);
      },
    });
  
    this.subscriptions.add(subscription);
  }  

  onRunTranslation(task: TranslationTaskDto): void {
    console.log('Running translation for:', task);
  
    const subscription = this.translationService.runTranslation(task.id).subscribe({
      next: () => {
        console.log(`Translation started for task ${task.id}`);
        this.getTranslationTasks(); // Обновляем задачи сразу
      },
      error: (error) => {
        console.error('Translation failed:', error);
      },
    });
    
    setTimeout(() => {
      this.getTranslationTasks();
    }, 3000);

    this.subscriptions.add(subscription);
  }
  
  private startTranslationTasksPolling(): void {
    if (this.pollingSubscription) return; // Не запускаем пулинг, если он уже запущен
  
    this.pollingSubscription = interval(this.updateTranslationTasksInterval)
      .pipe(
        switchMap(() => this.translationService.getTranslationTasks(this.selectedUserDocumentId!)),
        takeWhile(() => this.runTranslationCount > 0),
        catchError((error) => {
          console.error('Error during polling tasks:', error);
          return of(null); // Возвращаем пустой поток при ошибке
        })
      )
      .subscribe({
        next: (tasks: TranslationTasksDto | null) => {
          if (tasks) {
            this.translationTasks = tasks;
            this.runTranslationCount = this.translationTasks?.translationTasks.filter(task => task.status === 'InProgress').length || 0;
          }
        },
        complete: () => {
          console.log('Polling completed.');
          this.stopTranslationTasksPolling();
        },
      });
  
    console.log('Polling started.');
  }
  
  private stopTranslationTasksPolling(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
      this.pollingSubscription = null;
      console.log('Polling stopped.');
    }
  }  
  
  onDownloadTranslation(task: TranslationTaskDto): void {
    if(!task.translatedDocumentId){
      return;
    }

    this.fileService.downloadFile(task.translatedDocumentId).subscribe({
      next: ({ blob, fileName }) => {
        // Создаём ссылку для скачивания
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
    
        // Устанавливаем имя файла из ответа сервера
        a.download = fileName;
    
        // Кликаем по ссылке для скачивания
        a.click();
    
        // Освобождаем объект URL
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading file:', error);
      },
    });
  }
  

  ngOnDestroy(): void {
    // Отмена всех подписок
    this.subscriptions.unsubscribe();
    this.stopTranslationTasksPolling();
  }
}
