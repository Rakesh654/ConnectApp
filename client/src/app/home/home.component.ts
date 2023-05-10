import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  registerMode= false;
  users: any;
  constructor()
  {

  }

  ngOnInit(): void
  {
  }
 
  registerToggle()
  {
    this.registerMode = !this.registerMode;
  }

  cancelregisterMode(event: boolean)
  {
    this.registerMode = false;
  }

}
