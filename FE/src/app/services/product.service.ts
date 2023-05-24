import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "@env/env";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(
    private http: HttpClient) { }

  getListProduct(lang: String, cateId: String, page: number, size: number): Observable<any> {
    const url = environment.urlAPI + 'api/Products/paging?' + "LanguageId=" + environment.language + `&CategoryId=${cateId}`
      + `&PageIndex=${page}` + `&PageSize=${size}`;
    return this.http.get(url);
  }
  getDetailProductById(id: string, langId: string) {
    const url = environment.urlAPI + 'api/Products/' + id + `/${langId}`;
    return this.http.get(url);
  }
}
