import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginationResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members : Member[] = [];
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;
  
  constructor(private http : HttpClient, private accountService: AccountService) {
    // this.accountService.currentUser$.pipe(take(1)).subscribe({
    //   next: user =>{
    //     if(user){
    //       this.userParams = new UserParams(user);
    //       this.user = user;
    //     }
    //   }
    // })
   }

   getUserParams(){
    const user = localStorage.getItem('user');
    if(user)
    var data = JSON.parse(user);
    this.userParams = new UserParams(data);
    this.user = data;
    return this.userParams;
   }

   setUserParams(params: UserParams){
    this.userParams = params;
   }

   resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }

    return;
   }

  getMembers(userParams : UserParams){
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    // if(this.members.length > 0) return of(this.members)
    return getPaginatedResult<Member[]>(this.baseUrl + 'user', params, this.http).pipe(map(response =>{
      this.memberCache.set(Object.values(userParams).join('-'), response);
      return response;
    }))
  }

  getMember(username: string){
    const member = [...this.memberCache.values()]
    .reduce((arr, element) => arr.concat(element.result), [])
    .find((member: Member) => member.userName === username);
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

  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize : number){
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }
}
