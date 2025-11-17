import { Injectable } from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Group} from '../../interfaces/group';
@Injectable({
  providedIn: 'root'
})
export class GroupService {

  private readonly basiceUrl = 'api/Group';
  constructor(private http: HttpClient) { }

  getGroups(): Observable<Group[]>{
    return this.http.get<Group[]>(this.basiceUrl);
  }
  getGroup(id:number){
    return this.http.get<Group>(this.basiceUrl+'/'+id);
  }

  createGroup(data: any): Observable<Group>{
    return this.http.post<Group>(this.basiceUrl,data);
  }

  updateGroup(data: any): Observable<Group>{
    return this.http.put<Group>(this.basiceUrl,data);
  }

  deleteGroup(id: number): Observable<void>{
    return this.http.delete<void>(this.basiceUrl+`/${id}`);
  }

  addUserToGroup(userId: number,groupId: number): Observable<void>{
    return this.http.post<void>(`${this.basiceUrl}/add-user/${userId}/${groupId}`,null);
  }

  removeUserFromGroup(userId: number,groupId: number) {
    return this.http.delete(this.basiceUrl + `/remove-user/${userId}/${groupId}`)
  }

  getGroupImagePath(id: number): Observable<Blob> {
    return this.http.get(`${this.basiceUrl}/image/${id}`, {
      responseType: 'blob'
    });
  }
  saveGroupImage(id: number, file: File, isUpdate: boolean): Observable<any> {
    const formData = new FormData();
    formData.append('groupId', id.toString());
    formData.append('image', file);
    formData.append('isUpdate', isUpdate.toString());

    return this.http.post(`${this.basiceUrl}/upload-image`, formData, {})
  }
}
