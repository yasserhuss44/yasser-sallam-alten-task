import { Component } from '@angular/core';
import { DashBoardService } from './dashboard.service';
import { ResponseDetailsResult, VehicleSearch } from './dashboard.models';
import { interval } from 'rxjs/observable/interval';
import { timer } from 'rxjs/observable/timer';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashBoardComponent {
  title = 'Customer Vehicles Dashboard';
  constructor(private dashboardService: DashBoardService) { }
  vehiclesList: ResponseDetailsResult = { itemsList: [] };
  searchTag = "";
  public loading = false;
  public autoRefreshEnabled = false;
  private  subscribe;
  ngOnInit() {
    this.loadAllVehicles();
  }
  loadAllVehicles(): void {
    console.log("Start");
    this.loading = true;
    this.dashboardService.search(this.searchTag).subscribe(vehiclesList => {
      //console.table(heroes.itemsList);
      this.vehiclesList = vehiclesList
      this.loading = false;
      //  this.refreshData();

    });
  }
 

  search(): void {
    this.loadAllVehicles();
  }

  keyDownFunction($event: any): void {
    if (event["keyCode"] == 13) {
      this.loadAllVehicles();
    }
  }
  toggleAutoRefresh() {
    this.autoRefreshEnabled = !this.autoRefreshEnabled;
    if (this.autoRefreshEnabled) {
      const source = timer(500, 20000);
      //output: 0,1,2,3,4,5......
      this.subscribe = source.subscribe(val => this.loadAllVehicles());
    }
    else {
      this.subscribe.unsubscribe();// = source.subscribe(val => this.loadAllVehicles());
    }
  }
}



// import { Injectable } from '@angular/core';
// import { HttpClient } from '@angular/common/http';
// import { Client } from '../model/client.model';

// @Injectable()
// export class ClientService {
//   constructor(private http: HttpClient) { }
//   url = 'http://localhost:8080/api/clients';

//   getAll() {
//     return this.http.get<Client[]>(this.url);
//   }

//   getById(id: number) {
//     return this.http.get<Client>(this.url + '/' + id);
//   }

//   create(Client: Client) {
//     return this.http.post(this.url, Client);
//   }

//   update(Client: Client) {
//     return this.http.put(this.url + '/' + Client.id, Client);
//   }

//   delete(id: number) {
//     return this.http.delete(this.url + '/' + id);
//   }
// }