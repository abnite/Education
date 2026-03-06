import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/auth.models';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="admin-users">
      <div class="page-header">
        <h1>👥 User Management</h1>
        <div class="search-box">
          <input [(ngModel)]="searchTerm" placeholder="🔍 Search users…" (input)="filterUsers()" />
        </div>
      </div>

      <div class="filter-tabs">
        <button [class.active]="roleFilter === ''" (click)="setFilter('')">All</button>
        <button [class.active]="roleFilter === 'Parent'" (click)="setFilter('Parent')">Parents</button>
        <button [class.active]="roleFilter === 'Learner'" (click)="setFilter('Learner')">Learners</button>
        <button [class.active]="roleFilter === 'Admin'" (click)="setFilter('Admin')">Admins</button>
      </div>

      <div class="users-table-wrap">
        <table class="users-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Role</th>
              <th>Joined</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let u of filtered()">
              <td>{{ u.firstName }} {{ u.lastName }}</td>
              <td>{{ u.email }}</td>
              <td><span class="role-badge" [class]="'role-' + u.role.toLowerCase()">{{ u.role }}</span></td>
              <td>{{ u.createdAt | date:'shortDate' }}</td>
              <td>
                <span class="status-badge" [class.active]="u.isActive" [class.inactive]="!u.isActive">
                  {{ u.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td>
                <button class="btn-icon" *ngIf="u.isActive" (click)="deactivate(u.id)" title="Deactivate">🚫</button>
                <button class="btn-icon" *ngIf="!u.isActive" (click)="activate(u.id)" title="Activate">✅</button>
              </td>
            </tr>
            <tr *ngIf="!filtered().length">
              <td colspan="6" class="empty-row">No users found.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styleUrls: ['./admin-users.component.scss']
})
export class AdminUsersComponent implements OnInit {
  private http = inject(HttpClient);

  users = signal<any[]>([]);
  filtered = signal<any[]>([]);
  searchTerm = '';
  roleFilter = '';

  ngOnInit() {
    this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/admin/users`).subscribe({
      next: res => { if (res.success) { this.users.set(res.data); this.filtered.set(res.data); } }
    });
  }

  filterUsers() {
    const q = this.searchTerm.toLowerCase();
    this.filtered.set(this.users().filter(u =>
      (!this.roleFilter || u.role === this.roleFilter) &&
      (!q || u.email.toLowerCase().includes(q) || (u.firstName + ' ' + u.lastName).toLowerCase().includes(q))
    ));
  }

  setFilter(role: string) {
    this.roleFilter = role;
    this.filterUsers();
  }

  deactivate(id: string) {
    this.http.post<ApiResponse<any>>(`${environment.apiUrl}/admin/users/${id}/deactivate`, {}).subscribe({
      next: () => this.users.update(list => list.map(u => u.id === id ? { ...u, isActive: false } : u))
    });
    this.filterUsers();
  }

  activate(id: string) {
    this.http.post<ApiResponse<any>>(`${environment.apiUrl}/admin/users/${id}/activate`, {}).subscribe({
      next: () => this.users.update(list => list.map(u => u.id === id ? { ...u, isActive: true } : u))
    });
    this.filterUsers();
  }
}
