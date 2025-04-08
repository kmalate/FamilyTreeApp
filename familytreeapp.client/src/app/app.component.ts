import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import FamilyTree from "@balkangraph/familytree.js";
import { FamilyService } from './services/family.service';
import { UpdateNodeArgs } from './models/update-node-args.model';
import { FamilyNode } from './models/family-node.model';
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
  familyTree: FamilyTree | null = null;
  constructor(private familyService: FamilyService) { }

  private clientNodeObjectToFamilyNode(nodes: any[]): FamilyNode[] {
    let familyNodes: FamilyNode[] = [];
    nodes.forEach((node: any) => {
      const familyNode: FamilyNode = {
        id: node.id,
        name: node.name ? node.name : "",
        mid: node.mid ? node.mid : "",
        fid: node.fid ? node.fid : "",
        gender: node.gender ? node.gender : "",
        pids: node.pids ? node.pids : [],
      };

      familyNodes.push(familyNode);
    });

    return familyNodes;
  } 

  ngOnInit() {
    const divTree = document.getElementById("tree");

    if (divTree) {
      this.familyTree = new FamilyTree(divTree, {
        mode: "dark",
        nodeTreeMenu: true,
        nodeBinding: {
          field_0: "name",
          field_1: "id"
        },
      });

      this.familyService.getFamilyNodes().subscribe((data) => {
        if (this.familyTree) {
          this.familyTree.load(data);
        }        
      });

      this.familyTree.onUpdateNode((args) => {
        // convert FamilyTreeJS args to UpdateNodeArgs
        const updateArgs: UpdateNodeArgs = {
          addNodesData: this.clientNodeObjectToFamilyNode(args.addNodesData),
          updateNodesData: this.clientNodeObjectToFamilyNode(args.updateNodesData),
          removeNodeId: args.removeNodeId,
        };

        this.familyService.updateFamilyTreeNodes(updateArgs).subscribe((response) => {
          this.familyTree?.replaceIds(response);
        });
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
