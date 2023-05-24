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
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  closeDrawer() {
    this.visible = !this.visible;
  }
  openCart() {
    this.visible = true;
  }

  ngAfterViewInit(): void {
    this.product.subscribe((res: any) => {
      if (res.id && this.listProductCarts.length !== 0) {
        this.listProductCarts.forEach((item) => {
          if (item.id == res.id) {
            item.quantity = item.quantity + res.quantity;
            this.getTotal();
          }
          else {
            this.listProductCarts.push(res);
            this.getTotal();
          }
        })
      }
      else if (res.id) {
        this.listProductCarts.push(res);
        this.getTotal();
        this.cdr.detectChanges();
      }
    })
  }


  test() {
    console.log(this.listProductCarts)
  }

  removeFromCart(i: number) {
    this.listProductCarts.splice(i,1);
  }
  getTotal(){
    this.listProductCarts.forEach(item => {
      if (item.id){
        this.total = this.total = Number(item.price) * Number(item.quantity);
      }
    })
  }

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
  }
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) {
      this.setOfCheckedId.add(id);
    } else {
      this.setOfCheckedId.delete(id);
    }
  }

  changeQuantiny(e: any, data: any) {
    data.quantity = e;
    this.getTotal();
  }
}

