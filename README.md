# BigTic
Small 2-player web-game. docker-compose.yml
<code>
<pre>
  services:
  bigtic:
    image: "leo2002zxc/bigtic"
    container_name: big_tic
    ports:
      - "8081:8081"
    environment:
      - DB_SERVER=sql-server2022
  sql:
    image: "mcr.microsoft.com/mssql/server:2022-latest"
    container_name: sql-server2022
    ports:
      - "1433:1433" 
    environment:
      - ACCEPT_EULA=y
      - SA_PASSWORD=A&VeryComplex123Password
</pre>
</code>
