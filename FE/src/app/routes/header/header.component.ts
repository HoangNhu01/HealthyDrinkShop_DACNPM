import {AfterViewInit, ChangeDetectorRef, Component, DoCheck, ViewChild} from '@angular/core';
import {CartComponent} from "../cart/cart.component";
import {NzMessageService} from "ng-zorro-antd/message";
import {JwtHelperService} from "@auth0/angular-jwt";
import {NzModalService} from "ng-zorro-antd/modal";
import {ProductService} from "../../services/product.service";
import {environment} from "@env/env";
import {BehaviorSubject, Subject} from "rxjs";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent {
  constructor(
    protected message: NzMessageService,
    protected productService: ProductService,
    protected cdr: ChangeDetectorRef
  ) {
  }
  name: any = ''
  @ViewChild('cart') cartComp! : CartComponent;
  numberProductInCart = 0;

  openCart() {
    this.cartComp.openCart();
  }

  checkAcc(): boolean {
    if (localStorage.getItem('name')) {
      this.name = localStorage.getItem('name');
      return true
    }
    else return false
  }

  logout() {
    localStorage.removeItem('name');
    localStorage.removeItem('email');
    localStorage.setItem('isRememberMe', 'false');
  }
  updateCount(num: number): void {
    this.numberProductInCart = num;
    this.cdr.markForCheck();
  }

}
