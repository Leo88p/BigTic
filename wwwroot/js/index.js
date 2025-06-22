const canvas = document.getElementById("canvas")
const user = canvas.dataset.user
const height = window.innerWidth > window.innerHeight ? window.innerHeight : window.innerWidth
canvas.width = height //set canvas width to its parent width
canvas.height = height
const ctx = canvas.getContext("2d")
ctx.strokeStyle = "green"
ctx.fillStyle = "white"
ctx.lineWidth = 5
ctx.font = "normal 20px sans-serif"
const size = 9
let game;
const rectSize = canvas.width * 0.95 / size 

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/move")
    .build();

async function onClick(e) {
    const rect = canvas.getBoundingClientRect()
    const x = (e.clientX - rect.left).toString().replace(".", ",")
    const y = (e.clientY - rect.top).toString().replace(".", ",")
    const r = rectSize.toString().replace(".", ",")
    const w = canvas.width.toString().replace(".", ",")
    hubConnection.invoke("Send", user, x, y, r, w)
}
function Surrender(e) {
    if (e.code == 'KeyZ') {
        hubConnection.invoke("Capitulate", user)
    }
}
function drawLine(arc, i, j, di, dj) {
    if (arc.value == "1") {
        ctx.lineWidth = 5
        ctx.strokeStyle = "red"
    } else {
        ctx.lineWidth = 2
        ctx.strokeStyle = "rgb(0, 255, 0)"
    }
    ctx.beginPath()
    ctx.moveTo(canvas.width * 0.025 + i * rectSize, canvas.width * 0.025 + j * rectSize)
    let destination
    destination = [canvas.width * 0.025 + di * rectSize, canvas.width * 0.025 + dj * rectSize]
    ctx.lineTo(...destination)
    ctx.stroke()
}
function render(gameString) {
    game = JSON.parse(gameString)
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    let playerName;
    if (!game.gameEnded) {
        if (game.player == 0) {
            ctx.fillStyle = "cyan"
            playerName = "синий"
        } else {
            ctx.fillStyle = "yellow"
            playerName = "жёлтый"
        }
        ctx.fillText("Ходит " + playerName, 5, 20)
    }
    else {
        if (game.playerSurrended != 0) {
            if (game.playerSurrended == 1) {
                ctx.fillStyle = "yellow"
                playerName = "Синий"
            }
            else {
                ctx.fillStyle = "cyan"
                playerName = "Жёлтый"
            }
            ctx.fillText(playerName + " сдался", 5, 20)
        }
        else {
            if (game.score[0] > game.score[1]) {
                ctx.fillStyle = "cyan"
                playerName = "Синий"
            } else {
                ctx.fillStyle = "yellow"
                playerName = "Жёлтый"
            }
            ctx.fillText(playerName + " победил", 5, 20)
        }
    }

    ctx.fillStyle = "cyan"
    let margin = (ctx.measureText("Ходит жёлтый").width - ctx.measureText(`${game.score[0]} : ${game.score[1]}`).width) / 2
    ctx.fillText(game.score[0], margin, 50)
    margin += ctx.measureText(game.score[0]).width
    ctx.fillStyle = "white"
    ctx.fillText(" : ", margin, 50)
    margin += ctx.measureText(" : ").width
    ctx.fillStyle = "yellow"
    ctx.fillText(game.score[1], margin, 50)
    if (game.User1 == user) {
        ctx.fillStyle = "cyan";
        ctx.fillText("Вы синий", 5, 80)
    } else {
        ctx.fillStyle = "yellow";
        ctx.fillText("Вы жёлтый", 5, 80)
    }
    if (!game.gameEnded) {
        ctx.fillStyle = "red";
        ctx.fillText("Нажмите Ζ чтобы сдаться", canvas.width - 250, 20)
    }
    for (let i = 0; i < size; ++i) {
        for (let j = 0; j < size; ++j) {
            if (game.field[i][j].value != "empty") {
                if (game.field[i][j].value == "0") {
                    ctx.fillStyle = "white"
                } else if (game.field[i][j].value == "1") {
                    ctx.fillStyle = "cyan"
                } else {
                    ctx.fillStyle = "yellow"
                }
                ctx.beginPath();
                ctx.rect(canvas.width * 0.025 + i * rectSize, canvas.width * 0.025 + j * rectSize, rectSize, rectSize)
                ctx.fill()
            }
        }
    }
    for (let i = 0; i < size + 1; i++) {
        for (let j = 0; j < size + 1; j++) {
            if (game.horizontalArcs[i][j].value != "empty") {
                drawLine(game.horizontalArcs[i][j], i, j, i + 1, j)
            }
            if (game.verticalArcs[i][j].value != "empty") {
                drawLine(game.verticalArcs[i][j], i, j, i, j + 1)
            }
        }
    }
    if (game.gameEnded) {
        canvas.removeEventListener('click', onClick);
        document.removeEventListener('keypress', Surrender);
    }
}
document.addEventListener('keypress', Surrender)
hubConnection.on("Recieve", (gameString) => render(gameString))
hubConnection.start().then(() => hubConnection.invoke("Send", user, "null", "null", "null", "null"))
canvas.addEventListener("click", onClick)