## requirements

- docker
- kubernetes
- ingress-nginx (https://kubernetes.github.io/ingress-nginx/)

## configuration

- add this to host file
```
127.0.0.1 dotnetmicrosrvtestapp.dev
```

- apply deployments and services

```
    kubectl apply -f command-depl.yaml
    kubectl apply -f platform-depl.yaml
    kubectl apply -f platform-np-srv.yaml
    kubectl apply -f ingress-srv.yaml
```