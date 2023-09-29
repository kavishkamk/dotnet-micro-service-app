## test dontent mocro service app

- dotnet micro service application
- sql server for database
- rabbitMQ for event bus
- Http Sync client
- Grpc Sync client
- ingress nginx for api gateway
- docker and kubernates for deployments

## architectre diagram

<img src="https://github.com/kavishkamk/dotnet-micro-service-app/blob/main/assets/architecture-diagram.png" alt="Architectur" title="Architecture Diagram">

## services

- platform service swagger:
  http://localhost:5284/swagger

- command services swagger :
  http://localhost:5172/swagger

- rabbitmq management studio
  http://localhost:15672/

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
    kubectl apply -f local-pvc.yaml
    kubectl apply -f rabbitmq-depl.yaml
```

- create secrets

```
    kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55word!"
```
