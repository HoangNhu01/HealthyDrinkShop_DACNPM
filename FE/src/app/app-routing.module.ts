import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {HomeComponent} from "./routes/home/home.component";
import {AboutUsComponent} from "./routes/about-us/about-us.component";
import {DetailProductComponent} from "./routes/detail-product/detail-product.component";
import {AllProductComponent} from "./routes/all-product/all-product.component";
import {AccountComponent} from "./routes/account/account.component";
import {BlogComponent} from "./routes/blog/blog.component";

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'about-us', component: AboutUsComponent},
  {path: 'detail-product', component: DetailProductComponent, data: {id: ''}},
  {path: 'all-product', component: AllProductComponent},
  {path: 'account', component: AccountComponent},
  {
    path: 'blog',
    component: BlogComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  declarations: [],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
