// file-upload.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { TranslationOptionsDto } from '../models/translation-options.model';
import { TranslationTasksDto, UserDocumentDto } from '../models/translation.model';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
   
  constructor(private readonly http: HttpClient) { }
  private readonly prefix = '/api/translation'  

  getTranslationOptions(): Observable<TranslationOptionsDto> {
    return this.http.get<TranslationOptionsDto>(this.prefix + '/get-translation-options').pipe(
      map(response => response)
    );
  }

  getUserDocuments(): Observable<UserDocumentDto[]> {
    return this.http.get<UserDocumentDto[]>(this.prefix + '/get-user-documents').pipe(
      map(response => response)
    );
  }

  getTranslationTasks(documentId: number): Observable<TranslationTasksDto> {
    let params = new HttpParams();
    params = params.append('documentId', documentId.toString());
    return this.http.get<TranslationTasksDto>(this.prefix + '/get-translation-tasks', { params }).pipe(
      map(response => response)
    );
  }

  translateDocument(request: FormData): Observable<void> {
    return this.http.post<void>(this.prefix + '/translate', request).pipe(
        map(response => response)
    );  
  }

  runTranslation(taskId: number): Observable<void> {
    return this.http.post<void>(this.prefix + '/run-translation', taskId).pipe(
        map(response => response)
    );  
  }
}