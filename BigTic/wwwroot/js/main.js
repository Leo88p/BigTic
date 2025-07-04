const container = document.querySelector('.text');
function resize() {
    const diff = container.clientHeight / container.clientWidth
    if (diff < 1) {
        document.querySelector('.main').style.setProperty('--custom-size', `${5 / diff ** 0.5}cqh`)
    }
    else {
        document.querySelector('.main').style.setProperty('--custom-size', `${5 * diff ** 0.5}cqw`)
    }
}
resize()
window.addEventListener('resize', resize)

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/join")
    .build();
const playButton = document.getElementById("play");
if (playButton) {
    playButton.addEventListener("click", () => {
        hubConnection.invoke("Join");
    })
}
const cancelButton = document.getElementById("cancel");
if (cancelButton) {
    cancelButton.addEventListener("click", () => {
        hubConnection.invoke("Cancel");
    })
}
hubConnection.on("StartGame", () => {
    location = "/Game";
})
hubConnection.start()