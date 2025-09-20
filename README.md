# POO

(Extensão do VS Code Ritwick Dey Live Server é necessário)

Para rodar o projeto:

dotnet build

dotnet run

-------------------------------------------------------------
Caso apareça o erro 
"error MSB4018:
Falha inesperada da tarefa "ApplyCompressionNegotiation".
System.ArgumentException: An item with the same key has already been added."

1°
Vá em "\POO-main\Projeto_POO-main\obj\Debug"

2°
Delete a pasta net9.0

3°
só rodar o comando 'dotnet build' novamente (até agora foi a única coisa que funcionou)
