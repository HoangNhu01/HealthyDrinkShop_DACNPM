import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {HomeComponent} from "./routes/home/home.component";
import {AboutUsComponent} from "./routes/about-us/about-us.component";
import {DetailProductComponent} from "./routes/detail-product/detail-product.component";
import {AllProductComponent} from "./routes/all-product/all-product.component";
import {AccountComponent} from "./routes/account/account.component";
import {BlogComponent} from "./routes/blog/blog.component";
import {PaymentsComponent} from "./routes/payments/payments.component";
import {NotFoundComponent} from "./routes/not-found/not-found.component";
import {OrderListComponent} from "./routes/order-list/order-list.component";

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'about-us', component: AboutUsComponent},
  {path: 'detail-product', component: DetailProductComponent, data: {id: ''}},
  {path: 'all-product', component: AllProductComponent},
  {path: 'all-product/:search', component: AllProductComponent},
  {path: 'account', component: AccountComponent},
  {path: 'account/:id', component: AccountComponent},
  {path: 'account/:id/:product/:total', component: AccountComponent},
  {path: 'order-list', component: OrderListComponent},
  {
    path: 'blog',
    component: BlogComponent
  },
  {
    path: 'payment',
    component: PaymentsComponent
  },
  {
    path: '404',
    component: NotFoundComponent
  },
  {
    path: '**',
    redirectTo: '/404',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  declarations: [],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
