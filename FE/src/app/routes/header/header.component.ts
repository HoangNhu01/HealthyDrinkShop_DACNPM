import {Component, ViewChild} from '@angular/core';
import {CartComponent} from "../cart/cart.component";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent {
  @ViewChild('cart') cartComp! : CartComponent;

  openCart() {
    this.cartComp.openCart();
  }
}
