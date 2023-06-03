import {AfterViewInit, ChangeDetectorRef, Component, Input, OnChanges, ViewChild} from '@angular/core';
import {BehaviorSubject, Observable} from "rxjs";
import {environment} from "@env/env";
import {arraysEqual} from "ng-zorro-antd/core/util";
import {ProductService} from "../../services/product.service";
import {PaymentsComponent} from "../payments/payments.component";
import {ModalButtonOptions, NzModalService} from "ng-zorro-antd/modal";
import {HeaderComponent} from "../header/header.component";

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.less']
})
export class CartComponent implements AfterViewInit{
  @ViewChild('payments') payments!: PaymentsComponent
  @ViewChild('header') header!: HeaderComponent
  constructor(
    protected cdr: ChangeDetectorRef,
    protected productService: ProductService,
    protected modal: NzModalService
  ) {
  }
  @Input() visible = false;
  @Input() product = new BehaviorSubject<any>([]);
  listProductCarts: any[] = [];
  productPayments: any[] = [];

  title = 'Cart';
  total: number = 0;
  unit = environment.unitMoney;
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  closeDrawer() {
    this.visible = !this.visible;
  }
  openCart(data? : any) {
    this.visible = true;
    if (data) {
      this.productService.addProductToCart(data.id, environment.language, data.quantity).toPromise().then((res) => {
        this.getOrderFromCart();
      })
    }
    else this.getOrderFromCart();
  }

  ngAfterViewInit(): void {
  }
  getOrderFromCart(): void {
    this.productService.getOrder().toPromise().then((res: any) => {
      if (res) {
        this.listProductCarts = res;
        this.productPayments = res;
        // this.header.updateCount(res.length());
        this.getTotal();
      }
    });
  }

  getTotal(){
    const total = 0;
    const totalMoney = this.productPayments.reduce((accumulator, currentValue) =>
      accumulator + (currentValue.price * currentValue.quantity), total
    )
    this.total = totalMoney;
  }

  onItemChecked(id: number,e: boolean): void {
    if (e){
      this.productPayments.push(this.listProductCarts.find(item => item.productId === id));
    }
    else {
      this.productPayments = this.listProductCarts.filter((item: any) => {
        return item.productId !== id;
      })
    }
      this.getTotal();
  }

  changeQuantiny(data: any, isRemoveProduct?: boolean, e?: any) {
    // todo: cái này với getTotal nhiều bug vl
    debugger
    const quantiny = (isRemoveProduct || e === 0) ? 0 : e;
    this.productService.patchOrder(data.productId, quantiny).toPromise().then((res: any) => {
      if (res) {
        this.listProductCarts = res;
        this.productPayments = res;
        this.getTotal();
      }
    })
  }

  payment() {
    const isLogin = localStorage.getItem('name') ? true : false
    this.closeDrawer();
    if (!isLogin) {
      this.modal.warning({
        nzTitle: 'Chú ý!',
        nzContent: 'Đăng nhập tài khoản người dùng để thanh toán',
        nzOnOk: () => this.payments.openDrawer([], isLogin)
      })
    }else this.payments.openDrawer([], isLogin)
  }
}

