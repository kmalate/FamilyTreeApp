import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import FamilyTree from "@balkangraph/familytree.js";
import { FamilyService } from './services/family.service';
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  constructor(private familyService: FamilyService) {}

  ngOnInit() {
    const divTree = document.getElementById("tree");

    if (divTree) {
      var family = new FamilyTree(divTree, {
        mode: "dark",
        nodeTreeMenu: true,
        nodeBinding: {
          field_0: "name",
          field_1: "id"
        },
      });

      this.familyService.getFamilyNodes().subscribe((data) => {
        family.load(data);
      });

      family.onUpdateNode((args) => {
        //TODO: sync changes to database
        console.log(args);
      });

      //family.load([
      //  { id: 1, pids: [2], name: "Amber McKenzie", gender: "female", img: "https://cdn.balkan.app/shared/2.jpg" },
      //  { id: 2, pids: [1], name: "Ava Field", gender: "male", img: "https://cdn.balkan.app/shared/m30/5.jpg" },
      //  { id: 3, mid: 1, fid: 2, name: "Peter Stevens", gender: "male", img: "https://cdn.balkan.app/shared/m10/2.jpg" },
      //  { id: 4, mid: 1, fid: 2, name: "Savin Stevens", gender: "male", img: "https://cdn.balkan.app/shared/m10/1.jpg" },
      //  { id: 5, mid: 1, fid: 2, name: "Emma Stevens", gender: "female", img: "https://cdn.balkan.app/shared/w10/3.jpg" }
      //]);
    }
  }

  title = 'familytreeapp.client';
}
