import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import FamilyTree from "@balkangraph/familytree.js";
import { FamilyService } from './services/family.service';
import { UpdateNodeArgs } from './models/update-node-args.model';
import { FamilyNode } from './models/family-node.model';

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
        nodeMenu: {
          remove: { text: "Remove" },
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
    }
  }

  title = 'familytreeapp.client';
}
