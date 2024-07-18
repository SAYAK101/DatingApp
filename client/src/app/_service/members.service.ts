import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member';
import { AccountService } from './account.service';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/Photo';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private accountService = inject(AccountService);
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + `users`).subscribe({
      next: (response) => this.members.set(response),
    }); //, this.getHttpOptions());
  }

  getMember(username: string) {
    const member = this.members().find((x) => x.userName === username);
    if (member !== undefined) return of(member);

    return this.http.get<Member>(this.baseUrl + `users/` + username); //this.getHttpOptions());
  }

  updateMember(member: Member) {
    return this.http.put<Member>(this.baseUrl + 'users', member).pipe(
      tap(() => {
        this.members.update((members) =>
          members.map((m) => (m.userName === member.userName ? member : m))
        );
      })
    );
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.baseUrl + 'users/set-main-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photoUrl = photo.url;
              }

              return m;
            })
          );
        })
      );
  }

  deletePhoto(photoId: number) {
    return this.http
      .delete(this.baseUrl + 'users/delete-photo/' + photoId)
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.find((p) => p.id === photoId)) {
                m.photos = m.photos.filter((p) => p.id !== photoId);
              }

              return m;
            })
          );
        })
      );
  }

  // getHttpOptions(){
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization : `Bearer ${this.accountService.currentUser()?.token}`
  //     })
  //   }
  // }
}
