# Apanha Bolas - Picker 3D

Um clone de Picker 3D feito em Unity

![Picker3D](/picker3d.png)

## Aula

Link para vídeo aula aqui: 

## Introdução

Estamos aqui com mais um clone! Como eu ja disse antes, criar um clone é sempre uma forma bacana de treinar. Dessa vez eu escolhi clonar o _Picker 3D_

É um jogo em que você controla um apanhador e pega as bolinhas no caminho. Cada fase tem um mínimo de bolinhas que você precisa pegar, e desde que você pegue mais que esse mínimo, passa de fase.

Como vocês podem ver aí nas imagens do jogo, ele é bem simpleszinho, perfeito pra quem tá começando a desenvolver jogos!

## Organizando o Projeto

Muito bem tenho aqui uma cena vazia e dessa vez eu vou fazer tudo dentro do Unity! Pensando um pouco no _Picker 3D_ eu já sei de prima que vou precisar de uma pista e de um apanhador. A gente pode fazer isso com esses modelos básicos que vem com o Unity, indo na hierarquia com __botão direito > 3D Object > Cube__

Tudo o que vc precisa fazer é esticar o cubo, criar novos cubos e esticar etc para formar algo que parece uma pista. Para adiantar esse processo eu já fiz sem gravar uma pista e um apanhador. O apanhador é a mesma coisa, só fui colando os retângulos até fazer esse U. Só lembra de colocar __Collision Detection__ no rigidbody do pai em __Continuous Dynamic__ para ter uma colisão mais precisa, e marca também o _isKinematic_ como __true__.

![collision](/collision.png)

Então nesse jogo a gente precisa:

1. Fazer a câmera seguir o jogador
2. Fazer o jogador andar para frente sozinho
3. Colocar algumas bolinhas pro jogador coletar
4. Controlar o jogador clicando e pressionando o botão esquerdo do mouse
5. Adicionar um ponto de coleta das bolinhas!

## Câmera

Vou só fazer um nível aqui rapidinho, repetindo a pista. Depois, adicionamos um código _CameraComp.cs_ na câmera para fazer ela seguir o jogador. Poderia simplesmente colocar dentro do jogador mas assim ela iria se mover quando o jogador fosse pro lado. __Queremos que ela apenas siga o eixo X aqui__


```cs
public class CameraComp : MonoBehaviour
{
    [SerializeField]
    private GameObject _jogador;
    void LateUpdate()
    {
        transform.position = new Vector3(_jogador.transform.position.x+4, transform.position.y, transform.position.z);
    }
}
```

## Controlador do Apanhador

Agora, vamos fazer o jogador andar sozinho pra frente! Adicionamos o código _ControladorComp.cs_


```cs
public class ControladorComp : MonoBehaviour
{
    [SerializeField]
    private float _velocidade = 1f;

    void Update()
    {
        var x = (_velocidade * Time.deltaTime) * -1;
        transform.Translate(new Vector3(x, 0, 0));
    }
}
```

Isso aqui está __ERRADO__! Mas pera pera pera, se eu voltar pro _Unity_ e der _play_, o apanhador tá se movendo!

__Mas, isso tá errado__. Mais específicamente, o _transform.Translate_ está errado. Você sabe dizer o que tá errado no código? Já respondo!

Vamos agora adicionar as bolinhas. Para isso vou na hierarquia com __botão direito > 3D Object > Sphere__. Vou diminuir o tamanho dela (0.1, e o y para ficar bem perto da pista (0.3)). Adiciona um Rigidbody nela e coloca o _Collision detection_ como _Continuous Dynamic_

Vou criar um prefab dela e repetir ela na pista. Isso aqui vai ter um problema... Conforme eu for acumulando bolinhas, elas tendem a subir uma em cima da outra o que vai fazer com que eu perca algumas bolinhas

Então a gente precisa travar elas nesse eixo Y. E a gente poderia fazer isso usando essa opção Constraint do Rigidbody, mas se fizermos isso elas não vão cair no cesto mais tarde quando completarmos a fase. Temos então que criar um pequeno código:

## Esfera

```cs
public class EsferaComp : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y > 0.3f)
            transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
    }
}
```

## Consertando Colisão

Esse código aqui também tá errado, diga-se de passagem. Aliás, falando nisso, porque é que o código do apanhador e das bolinhas está errado? Parece estar tudo funcionando.

Sim, parece. Mas olha o que acontece se eu aumentar a velocidade do apanhador. Ooops!

O que acontece aqui é que o transform.position modifica diretamente o objeto de jogo, e não o Rigidbody, que é o que define a interação da física da engine. O Rigidbody tenta acompanhar, mas quando estamos movendo numa velocidade maior ele passa a ficar pra trás. Se ele está atrasado, as colisões não acontecem de forma correta e você começa a ver essas coisas esquisitas acontecerem.

Para resolver isso, só temos que mover o objeto pelo rigidbody!

```cs
void FixedUpdate() // Como estamos alterando o Rigidbody, isso afeta diretamente a física. Nesses casos é melhor usar FixedUpdate
{
    var x = (_velocidade * Time.fixedDeltaTime) * -1; // Mesmo de antes
    _rb.MovePosition(transform.position + new Vector3(x, 0, 0)); // Usamos MovePosition
}
```

E pronto, posso aumentar mais a velocidade e tudo vai ficar bem

## Sistema de Mouse Drag

Agora para fazer o apanhador arrastar com o mouse, precisamos fazer um sistema de arrastar mouse assim:

```cs
private float _objZPos;
private float _movZ;

// Detecta quando o mouse é clicado no objeto
private void OnMouseDown()
{
    // Obtém a posição Z do objeto na câmera
    _objZPos = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
}

// Detecta quando arrastamos o mouse
private void OnMouseDrag()
{
    // Obtém a posiçao do mouse
    Vector3 mousePoint = Input.mousePosition;
    // Troca a posição Z para a do objeto para saber a profundidade correta do clique
    mousePoint.z = _objZPos;
    // Salva o ponto de clique numa variável
    _movZ = Camera.main.ScreenToWorldPoint(mousePoint).z;
}
```

E agora atualizamos a posição do apanhador:

```cs
void FixedUpdate()
{
    var x = (_velocidade * Time.fixedDeltaTime) * -1;
    var novaPos = transform.position + new Vector3(x, 0, 0);
    novaPos.z = _movZ;
    _rb.MovePosition(novaPos);
}
```

Mas isso não limita o apanhador a ficar dentro da pista

```cs
void FixedUpdate()
{
    var x = (_velocidade * Time.fixedDeltaTime) * -1;
    var novaPos = transform.position + new Vector3(x, 0, 0);
    if(_movZ > 0.9f)
        novaPos.z = 0.9f;
    else if(_movZ < -1.9f)
        novaPos.z = -1.9f;
    else
        novaPos.z = _movZ;
    _rb.MovePosition(novaPos);
}
```

## Pontuação

Agora vamos abaixar uma das pistas e colocar um empty object com um collider. A gente vai usar esse collider para contar as bolinhas!

Vou adicionar um texto para contar as bolinhas, vou adicionar aqui um TextMeshPro. Isso adiciona também um Canvas. Altere Render mode para 'World Space', e vamos rodar ele e diminuir

Agora que temos um contador, vamos fazer um código para contar as bolinhas

```cs
public class Pontuacao : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _txt;

    private int _pontos;

    // Start is called before the first frame update
    void Start()
    {
        _pontos = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        _pontos++;
        _txt.text = _pontos + " / 15";
    }

}
```


E pronto! Temos um Picker 3D funcional em apenas alguns minutos!