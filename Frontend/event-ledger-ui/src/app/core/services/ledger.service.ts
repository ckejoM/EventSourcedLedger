import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// This interface mirrors our C# AccountDashboardView exactly
export interface AccountDashboardView {
  id: string;
  ownerName: string;
  currentBalance: number;
  totalTransactions: number;
  lastUpdatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class LedgerService {
  // IMPORTANT: Replace this port with your actual backend HTTPS port (check your .http file or terminal)
  private apiUrl = 'https://localhost:7199/api/accounts'; 

  constructor(private http: HttpClient) { }

  openAccount(ownerName: string): Observable<{ accountId: string }> {
    return this.http.post<{ accountId: string }>(this.apiUrl, { ownerName });
  }

  deposit(accountId: string, amount: number, reference: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${accountId}/deposit`, { amount, reference });
  }

  getDashboard(accountId: string): Observable<AccountDashboardView> {
    return this.http.get<AccountDashboardView>(`${this.apiUrl}/${accountId}`);
  }
}