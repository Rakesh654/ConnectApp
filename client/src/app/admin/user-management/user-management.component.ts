import { Component } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent {
  users : User[] = []
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>;
  availableRoles : string[] =[
    'Admin',
    'Moderator',
    'Member'
  ]
  constructor(private adminService : AdminService, private modalSerive : BsModalService){}

  ngOnInit(): void{
    this.getUserWithRoles();
  }

  getUserWithRoles(){

    this.adminService.getUserWithRoles().subscribe({
      next: user => this.users = user
    })
  }

  openRolesModal(user : User){
    const config ={
      class: 'modal-dailog-centered',
      initialState: {
        username:user.username,
        availableRoles:this.availableRoles,
        selectedRoles: [...user.roles]
      }
    }
    this.bsModalRef = this.modalSerive.show(RolesModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next : () =>{
          const selectedRoles = this.bsModalRef.content?.selectedRoles;
          if(!this.arrayEqual(selectedRoles!, user.roles)){
            this.adminService.updateUserRole(user.username, selectedRoles!).subscribe({
              next: roles => user.roles = roles
            })
          }
      }
    })
  }

  private arrayEqual(arr1: any[], arr2: any[]){
      return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }
}
