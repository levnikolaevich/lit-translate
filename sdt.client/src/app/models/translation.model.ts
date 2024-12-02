export interface TranslateDocumentRequest {
    selectedFile: File | undefined;
    targetLanguageIds: (number)[];
    languageModelId?: number;
    apiKey?: string;
}

export interface UserDocumentDto
{
    id: number;
    name: string;
}

export interface TranslationTasksDto {
    documentInfo: DocumentInfoDto; // Required Document Info
    translationTasks: TranslationTaskDto[]; // Array of translation tasks
  }
  
  export interface DocumentInfoDto {
    uploadDateTime: string; 
    name: string; // Name of the document
    tokenCount: number; // Total token count
    price: number; // Total price
    progress: number; // Progress (as percentage or value)
  }
  
  export interface TranslationTaskDto {
    id: number;
    status: string; 
    prompt: string; // Prompt used for translation
    targetLanguage: string; // Target language for translation
    tokenCount: number; // Token count for this task
    price: number; // Price for this task
    progress: number; // Progress for this task
    translatedDocumentId: number | undefined;
  }  