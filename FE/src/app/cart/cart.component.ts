import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.less']
})
export class CartComponent {
  @Input() visible = false;
  title = 'Giỏ hành (1)';

  close() {
    this.visible = !this.visible;
  }
  openCart() {
    this.visible = true;
  }
}
