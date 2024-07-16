import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from "../_models/Member";
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private accountService = inject(AccountService);

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + `users`);//, this.getHttpOptions());
  }

  getMember(username : string) {
    return this.http.get<Member>(this.baseUrl + `users/` + username); //this.getHttpOptions());
  }

  // getHttpOptions(){
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization : `Bearer ${this.accountService.currentUser()?.token}`
  //     })
  //   }
  // }
}
