import { FamilyNode } from "./family-node.model";

export interface UpdateNodeArgs {
  removeNodeId: string | number;
  updateNodesData: FamilyNode[];
  addNodesData: FamilyNode[];
}
