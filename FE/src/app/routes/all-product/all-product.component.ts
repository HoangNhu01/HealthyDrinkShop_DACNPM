import {Component, OnInit} from '@angular/core';
import {ProductService} from "../../services/product.service";
import {ActivatedRoute, Router, Routes} from "@angular/router";
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
  searchValue: any = null
  constructor(
    protected productService: ProductService,
    protected router: Router,
    protected activatedRoute: ActivatedRoute
  ) {
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((res: any) => {
      this.searchValue = res['search']
      this.isLoading = true;
      this.getListProducts().catch(() => {
        this.isLoading = false;
      })
    })
    this.isLoading = true;
    this.getListProducts().catch(() => {
      this.isLoading = false;
    })
    }
  async getListProducts() {
    await this.productService.getListProduct(environment.language, '1', 1, 10).toPromise().then((res: any) => {
      if (res) {
        this.listProducts = res.resultObj.items;
        if (this.searchValue) {
          this.listProducts = this.listProducts.filter((item: any) => {
            return this.removeVietnameseTones(item.name).includes(this.removeVietnameseTones(this.searchValue))
          })
        }
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
  removeVietnameseTones(str: string): string {
    // Chuyển đổi chữ hoa thành chữ thường
    str = str.toLowerCase();

    // Các ký tự đặc biệt trong tiếng Việt và chữ thường tương ứng
    const fromChars = "àáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđ";
    const toChars =   "aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd";

    // Thực hiện chuyển đổi ký tự
    let newStr = "";
    for(let i=0; i<str.length; i++) {
      const index = fromChars.indexOf(str[i]);
      if(index >= 0) {
        newStr += toChars[index];
      } else {
        newStr += str[i];
      }
    }

    return newStr;
  }
}
