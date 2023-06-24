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
  addProductToCart(id: string, langId: string, quantity: number){
    const url = environment.urlAPI + 'api/Orders/' + id + `/${langId}` + '?clientQuantity=' + quantity;
    return this.http.post(url, {});
  }
  getOrder() {
    const url = environment.urlAPI + 'api/Orders/' ;
    return this.http.get(url);
  }
  patchOrder(id: string, quantity: number) {
    const url = environment.urlAPI + 'api/Orders/' + id + "/" + quantity ;
    return this.http.patch(url, {});
  }
  login(body: any) {
    const url = environment.urlAPI + 'api/Users/authenticate' ;
    return this.http.post(url, body);
  }
  payments(body: any) {
    const url = environment.urlAPI + 'api/Orders/checkouts' ;
    return this.http.post(url, body);
  }
  getProvinces() {
    const url = 'https://provinces.open-api.vn/api/?depth=3' ;
    return this.http.get(url);
  }
  eWalletPayments(body: {}) {
    const url = environment.urlAPI + 'api/Payments';
    return this.http.post(url, body);
  }
  registry(body: object) {
    const url = environment.urlAPI + 'api/Users';
    return this.http.post(url, body);
  }
}
