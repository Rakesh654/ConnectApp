import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members : Member[] = [];
  constructor(private http : HttpClient) { }

  getMembers(){
    if(this.members.length > 0) return of(this.members)
    return this.http.get<Member[]>(this.baseUrl + 'user').pipe(
      map(member => {
        this.members = member;
        return this.members;
      })
    )
  }

  getMember(username: string){
    const member = this.members.find(x => x.userName = username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'user/' + username)
  }

  updateMember(user : Member){
    return this.http.put(this.baseUrl + 'user',  user).pipe(
      map(() =>
        {
         const index = this.members.indexOf(user); 
         this.members[index] = {...this.members[index], ...user} 
        })
    ); 
  }

  setMainPhoto(photoId : number)
  {
      return this.http.put(this.baseUrl + 'user/set-main-photo/' + photoId, {});
  }

  deletephoto(photoId : number)
  {
      return this.http.delete(this.baseUrl + 'user/delete-photo/' + photoId, {});
  }
}
