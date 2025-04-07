namespace FamilyTreeApp.Server.Core.Models
{
    public class UpdateNodeArgsDTO
    {
        public int? RemoveNodeId { get; set; } = null;
        public FamilyNodeDTO[] UpdateNodesData { get; set; } = Array.Empty<FamilyNodeDTO>();
        public FamilyNodeDTO[] AddNodesData { get; set; } = Array.Empty<FamilyNodeDTO>();
    }
}


/*
{
    "removeNodeId": null,
    "updateNodesData": [
        {
            "id": "1",
            "pids": [
                2
            ],
            "mid": null,
            "fid": null,
            "name": "Gomez Addams",
            "gender": "maless"
        }
    ],
    "addNodesData": []
}

{
    "addNodesData": [
        {
            "mid": "_iwo5",
            "fid": "_l6ro",
            "gender": "male",
            "id": "_eyca"
        },
        {
            "pids": [
                "_l6ro"
            ],
            "gender": "female",
            "id": "_iwo5"
        }
    ],
    "updateNodesData": [
        {
            "mid": 4,
            "fid": "_p9wd",
            "gender": "male",
            "id": "_l6ro",
            "pids": [
                "_iwo5"
            ]
        }
    ],
    "removeNodeId": null
} 

 */