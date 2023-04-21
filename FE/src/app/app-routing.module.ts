import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {AboutUsComponent} from "./about-us/about-us.component";
import {DetailProductComponent} from "./detail-product/detail-product.component";
import {AllProductComponent} from "./all-product/all-product.component";
import {AccountComponent} from "./account/account.component";

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'about-us', component: AboutUsComponent },
  { path: 'detail-product', component: DetailProductComponent },
  { path: 'all-product', component: AllProductComponent },
  { path: 'account', component: AccountComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  declarations: [
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
