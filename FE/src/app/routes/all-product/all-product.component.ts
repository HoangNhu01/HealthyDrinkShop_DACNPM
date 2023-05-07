import {Component, OnInit} from '@angular/core';
import {ProductService} from "../../services/product.service";

@Component({
  selector: 'app-all-product',
  templateUrl: './all-product.component.html',
  styleUrls: ['./all-product.component.less']
})
export class AllProductComponent implements OnInit{
  constructor(
    protected productService: ProductService
  ) {
  }

  ngOnInit(): void {
    this.productService.getListProduct('vi-VN', '1', 1, 10).toPromise().then(res => {
      console.log(res);
    })

    }
}
