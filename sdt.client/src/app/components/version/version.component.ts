import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-version',
  template: '<p style="padding: 100px;">{{ version }}</p>',
})
export class VersionComponent implements OnInit {
  version: string | null = null;

  constructor(private readonly http: HttpClient) {}

  ngOnInit(): void {
    this.http.get('api/version/get-version', { responseType: 'text' }).subscribe({
      next: (data) => {        
        this.version = data;
      },
      error: (error) => {
        console.error('Error fetching version:', error);
        this.version = 'Error fetching version';
      },
    });
  }
}