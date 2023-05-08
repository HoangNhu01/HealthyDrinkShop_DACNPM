import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subscription, take} from "rxjs";
import {CartComponent} from "../cart/cart.component";
import {ProductService} from "../../services/product.service";
import {environment} from "@env/env";

@Component({
  selector: 'app-detail-product',
  templateUrl: './detail-product.component.html',
  styleUrls: ['./detail-product.component.less']
})
export class DetailProductComponent implements OnInit, OnDestroy {
  likes = 0;
  dislikes = 0;
  // time = formatDistance(new Date(), new Date());

  like(): void {
    this.likes = 1;
    this.dislikes = 0;
  }

  dislike(): void {
    this.likes = 0;
    this.dislikes = 1;
  }
  data: any[] = [];
  submitting = false;
  user = {
    author: 'Han Solo',
    avatar: 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'
  };
  inputValue = '';
  @ViewChild('cart') cart!: CartComponent;
  emptyImage: string = './assets/product/empty.png';
  id = ''
  sub : Subscription[] = [];
  detailProduct : any = {}
  quantity = 1;
  urlImage: string = './assets/product/';
  constructor(
    private route: ActivatedRoute,
    protected productService: ProductService
  ) {
  }

  ngOnInit() {
    let getIdProcess =  this.route.params.pipe(take(1)).subscribe((res: any) => {
      this.id = res.id ;
      this.productService.getDetailProductById(this.id, environment.language).toPromise().then((res: any) => {
        if (res) {
          this.detailProduct = res;
        }
      })
    })
    this.sub.push(getIdProcess);
  }
  ngOnDestroy() {
    this.sub.forEach(sub => sub.unsubscribe());
  }

  addToCart(data: any, quantity: number) {
    this.cart.product.next(data);
    this.cart.openCart();
  }
}
