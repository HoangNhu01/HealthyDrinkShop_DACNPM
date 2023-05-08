import {AfterViewInit, ChangeDetectorRef, Component, Input, OnChanges} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "@env/env";
import {arraysEqual} from "ng-zorro-antd/core/util";

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.less']
})
export class CartComponent implements AfterViewInit{
  constructor(
    protected cdr: ChangeDetectorRef
  ) {
  }
  @Input() visible = false;
  @Input() product = new BehaviorSubject<any>([]);
  listProductCarts: any[] = [];

  title = 'Cart';
  total: number = 0;
  unit = environment.unitMoney;

  closeDrawer() {
    this.visible = !this.visible;
  }
  openCart() {
    this.visible = true;
  }

  ngAfterViewInit(): void {
    this.product.subscribe((res: any) => {
      if (res.id) {
        this.listProductCarts.push(res);
        this.cdr.detectChanges();
      }
    })
  }


  test() {
    // console.log(this.listProductCarts)
  }

  removeFromCart(i: number) {
    this.listProductCarts.splice(i,1);
  }
}
