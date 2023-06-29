import {Component, OnInit} from '@angular/core';
import {environment} from "@env/env";
import {ProductService} from "../../services/product.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-best-product',
  templateUrl: './best-product.component.html',
  styleUrls: ['./best-product.component.less']
})
export class BestProductComponent implements OnInit {
  listProductsBestSaller: any[] = []
  emptyImage: string = './assets/product/empty.png';
  constructor(
    protected productService: ProductService,
    protected router: Router
  ) {
  }

  ngOnInit() {
    this.getProduct()
  }

  getProduct() {
    this.productService.getListProduct(environment.language, '1', 1, 10).toPromise().then((res: any) => {
      if (res) {
        this.listProductsBestSaller = res.resultObj.items.slice(0, 4);
      }
    })
  }

  getImage(base64: any): string {
    if (base64)
      return `data:image/jpeg;base64, ${base64} `;
    else return this.emptyImage;
  }
  getDetailProduct(id: string) {
    this.router.navigate(['detail-product', {id: id}]);
  }
}
