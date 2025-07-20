import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-version',
  template: `
    <div class="version-container">
      <div class="version-card">
        <div class="version-icon">
          <span>🚀</span>
        </div>
        <h2>Smart Document Translator</h2>
        <div class="version-info">
          <span class="version-label">Версия:</span>
          <span class="version-value">{{ version || 'Загрузка...' }}</span>
        </div>
        <div class="version-description">
          Система умного перевода документов с использованием ИИ
        </div>
      </div>
    </div>
  `,
  styles: [`
    .version-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
    }
    
    .version-card {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-radius: 24px;
      padding: 3rem;
      text-align: center;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.1);
      max-width: 400px;
      width: 100%;
      animation: fadeInUp 0.8s ease-out;
    }
    
    .version-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }
    
    h2 {
      color: #2d3748;
      font-size: 1.5rem;
      font-weight: 700;
      margin-bottom: 2rem;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }
    
    .version-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #f8fafc;
      padding: 1rem;
      border-radius: 12px;
      margin-bottom: 1.5rem;
    }
    
    .version-label {
      font-weight: 600;
      color: #64748b;
    }
    
    .version-value {
      font-weight: 700;
      color: #667eea;
      font-family: 'Courier New', monospace;
    }
    
    .version-description {
      color: #64748b;
      font-size: 0.9rem;
      line-height: 1.5;
    }
    
    @keyframes fadeInUp {
      from {
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
  `]
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