// file-upload.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileService {
   
  constructor(private readonly http: HttpClient) { }
  private readonly prefix = '/api/file'  
  
  downloadFile(documentId: number): Observable<{ blob: Blob; fileName: string }> {
    let params = new HttpParams().append('documentId', documentId.toString());
  
    return this.http.get(this.prefix + '/download', {
      params,
      responseType: 'blob',
      observe: 'response', // Получаем весь ответ, чтобы обработать заголовки
    }).pipe(
        map((response: HttpResponse<Blob>) => {
        const contentDisposition = response.headers.get('content-disposition');
        let fileName = `Document_${documentId}.docx`; // Имя по умолчанию
  
        if (contentDisposition) {
          const match = contentDisposition.match(/filename="(.+)"/);
          if (match && match[1]) {
            fileName = match[1]; // Имя файла из заголовка
          }
        }
  
        return { blob: response.body as Blob, fileName };
      })
    );
  }
}