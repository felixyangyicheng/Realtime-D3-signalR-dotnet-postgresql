import * as signalR from '@microsoft/signalr'

const signal = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:44350/loghub/', {})
    .build()

/* const signalr = function () {
  var hub
  if (hub === undefined) {
    hub = signal
  }
  return hub
} */
//  自动重连
/* async function start () {
  try {
    await signal.start()
    console.log('connected')
  } catch (err) {
    console.log(err)
    setTimeout(() => start(), 5000)
  }
}
 
signal.onclose(async () => {
  await start()
}) */

export default {

    install: function(Vue) {
        Vue.prototype.signalr = signal
    }
}