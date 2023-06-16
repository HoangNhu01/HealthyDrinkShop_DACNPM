import {Component, OnInit} from '@angular/core';
import {ProductService} from "../../services/product.service";
import {Router, Routes} from "@angular/router";
import en from "@angular/common/locales/en";
import {environment} from "@env/env";

@Component({
  selector: 'app-all-product',
  templateUrl: './all-product.component.html',
  styleUrls: ['./all-product.component.less']
})
export class AllProductComponent implements OnInit{
  listProducts : any[] = []
  urlImage: string = './assets/product/';
  emptyImage: string = './assets/product/empty.png';
  isLoading = false;
  constructor(
    protected productService: ProductService,
    protected router: Router
  ) {
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.productService.getListProduct(environment.language, '1', 1, 10).toPromise().then((res: any) => {
      if (res) {
        this.listProducts = res.resultObj.items;
      }
    }).finally(() => {
      this.isLoading = false;
    })
    }

  getDetailProduct(id: string) {
    this.router.navigate(['detail-product', {id: id}]);
  }

  getImage(base64: any): string {
    if (base64)
    return `data:image/jpeg;base64, ${base64} `;
    else return this.emptyImage;
  }
}
