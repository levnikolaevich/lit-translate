export interface TranslationOptionsDto {
    companies: AICompanyDto[];
    models: AILanguageModelDto[];
    languages: LanguageDto[];
  }
  
  export interface AICompanyDto {
    id: number;
    name: string;
  }
  
  export interface AILanguageModelDto {
    id: number;
    name: string;
    aiCompanyId: number;
    inputTokenPrice: number;
    outputTokenPrice: number;
    cachedTokenPrice: number;
  }
  
  export interface LanguageDto {
    id: number;
    code: string;
    name: string;
  }  