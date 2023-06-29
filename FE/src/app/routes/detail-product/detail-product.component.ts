import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription, take} from "rxjs";
import {CartComponent} from "../cart/cart.component";
import {ProductService} from "../../services/product.service";
import {environment} from "@env/env";
import {Guid} from "guid-typescript";

@Component({
  selector: 'app-detail-product',
  templateUrl: './detail-product.component.html',
  styleUrls: ['./detail-product.component.less']
})
export class DetailProductComponent implements OnInit, OnDestroy {
  listComments: any[] = []
  data: any[] = [];
  submitting = false;
  inputValue = '';
  @ViewChild('cart') cart!: CartComponent;
  emptyImage: string = './assets/product/empty.png';
  emptyAva: string = './assets/icon/trend-avatar-1.jpg';
  id = ''
  sub : Subscription[] = [];
  detailProduct : any = {}
  quantity = 1;
  urlImage: string = './assets/product/';
  unit = environment.unitMoney;
  isLogin = localStorage.getItem('userId') ? true : false
  visible = false;
  likes = 0;
  dislikes = 0;
  constructor(
    private route: ActivatedRoute,
    protected productService: ProductService,
    protected router: Router
  ) {
  }

  ngOnInit() {
    let getIdProcess =  this.route.params.pipe(take(1)).subscribe(async (res: any) => {
      this.id = res.id;
      await this.productService.getDetailProductById(this.id, environment.language).toPromise().then((res: any) => {
        if (res) {
          this.detailProduct = res.resultObj;
          this.getListComments();
        }
      })
    })
    this.sub.push(getIdProcess);
  }
  async getListComments() {
    const userId = localStorage.getItem('userId');
    const productId = this.detailProduct.id;
    const langid = environment.language;
    if (userId && productId && langid)
      await this.productService.getListComments(userId, productId, langid).toPromise().then((res: any) => {
        if (res && res.isSuccessed) {
          this.listComments = res.resultObj;
        }

      })
  }
  ngOnDestroy() {
    this.sub.forEach(sub => sub.unsubscribe());
  }

  addToCart(data: any, quantity: number) {
    data.quantity = quantity;
    this.cart.openCart(data);
  }

  getImage(listBase64: any): string {
    if (listBase64)
      return `data:image/jpeg;base64, ${listBase64[0]} `;
    else return this.emptyImage;
  }

  submit(parentId?: string) {
    if (!this.isLogin) {
      this.router.navigate(['/account', 3]);
    }
    else if (this.isLogin && this.inputValue) {
      const body = {
        id: Guid.create()['value'],
        userId: localStorage.getItem('userId'),
        commentText: this.inputValue,
        createdDate: new Date(),
        parentId: parentId ? parentId : null,
        productId: this.detailProduct.id
      }
      this.productService.addComments(body).toPromise().then((res: any) => {
        if (res && res.isSuccessed) {
          this.getListComments();
          this.inputValue = ''
        }
      })
    }
  }

  like(): void {
    // this.likes = 1;
    // this.dislikes = 0;
  }

  dislike(): void {
    // this.likes = 0;
    // this.dislikes = 1;
  }
}
