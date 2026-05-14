import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccountDashboardView, LedgerService } from '../../core/services/ledger.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  accountId: string | null = null;
  dashboard: AccountDashboardView | null = null;
  
  // Deposit Form Model
  depositAmount: number = 0;
  depositReference: string = '';

  constructor(private ledgerService: LedgerService) {}

  ngOnInit() {
    // For this project, we'll check localStorage for an existing account ID
    this.accountId = localStorage.getItem('ledger_account_id');
    if (this.accountId) {
      this.refreshDashboard();
    }
  }

  createNewAccount(ownerName: string) {
    this.ledgerService.openAccount(ownerName).subscribe(res => {
      this.accountId = res.accountId;
      localStorage.setItem('ledger_account_id', this.accountId);
      this.refreshDashboard();
    });
  }

  refreshDashboard() {
    if (this.accountId) {
      this.ledgerService.getDashboard(this.accountId).subscribe(data => {
        this.dashboard = data;
      });
    }
  }

  onDeposit() {
    if (this.accountId && this.depositAmount > 0) {
      this.ledgerService.deposit(this.accountId, this.depositAmount, this.depositReference)
        .subscribe(() => {
          this.depositAmount = 0;
          this.depositReference = '';
          this.refreshDashboard(); // Polling the read model after the command
        });
    }
  }
}