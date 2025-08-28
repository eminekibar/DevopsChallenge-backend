flowchart LR
  subgraph Client["Client (Browser)"]
    UI[Frontend UI]
  end

  subgraph K8s["Kubernetes Cluster (devopschallenge ns)"]
    subgraph FE["Frontend Deployment + HPA"]
      FEpods["Frontend Pods (svc: frontend:52369)"]
    end

    subgraph BE["Backend Deployment + HPA"]
      BEpods["Backend Pods (svc: backend:11130)"]
    end

    subgraph DB["PostgreSQL (StatefulSet + PVC)"]
      PG["postgres-0 (svc: postgres:5432)"]
    end
  end

  UI -->|HTTP| FEpods
  FEpods -->|HTTP (api/values)| BEpods
  BEpods -->|TCP 5432| PG

  classDef svc fill:#eef,stroke:#99f,stroke-width:1px,color:#000;
  class FEpods,BEpods,PG svc;
