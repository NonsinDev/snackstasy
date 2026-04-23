import './assets/main.css'

import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import App from './App.vue'
import router from './router'
import Button from 'primevue/button'
import { createPinia } from 'pinia'

import 'primeicons/primeicons.css'                     // Icons

import { VueQrcodeReader } from 'vue-qrcode-reader'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core'
import { faUser, faCamera } from '@fortawesome/free-solid-svg-icons'

library.add(faUser, faCamera)

const app = createApp(App)
const pinia = createPinia()
app.use(pinia)
app.use(PrimeVue)
app.use(router)
app.component('Button', Button)
app.use(VueQrcodeReader)
app.component('font-awesome-icon', FontAwesomeIcon)


app.mount('#app')