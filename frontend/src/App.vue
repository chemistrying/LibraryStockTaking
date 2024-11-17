<template>
    <navbar :pages="pages" :activePage="activePage" :nav-link-click="(index) => activePage = index"></navbar>
    <page-viewer :page="pages[activePage]"></page-viewer>
</template>

<script>
import PageViewer from './components/PageViewer.vue';
import Navbar from './components/Navbar.vue';

export default {
    // dynamic values = computed
    computed: {

    },
    components: {
        PageViewer, Navbar
    },
    // static data
    data() {
        return {
            apiUrl: '',
            activePage: 0, // 0 = root, 1 = whole session, 2 = bookshelf group, 3 = bookshelf
            sessionId: null,
            bookshelfGroup: null,
            bookshelfId: null,
            isAdmin: false,
            user: null,
            appPage: {
                ROOT: 0,
                SESSION: 1,
                GROUP: 2,
                BOOKSHELF: 3,
                PANEL: -1
            },
            operations: {
                ADD: "add",
                DELETE: "delete",
                START: "start",
                FINISH: "finish"
            },
            allSessionsInfo: {},
            windowWidth: window.innerWidth,
            pages: [
                {
                    link: { text: 'Home', url: 'index.html' },
                    pageTitle: 'Home Page',
                    content: 'This is the home content'
                },
                {
                    link: { text: 'Session', url: 'session.html' },
                    pageTitle: 'Session Page',
                    content: 'This is the session content'
                },
                {
                    link: { text: 'Bookshelf Group', url: 'group.html' },
                    pageTitle: 'Bookshelf Group Page',
                    content: 'This is the bookshelf group content'
                },
                {
                    link: { text: 'Bookshelf', url: 'bookshelf.html' },
                    pageTitle: 'Bookshelf Page',
                    content: 'This is the bookshelf content'
                }
            ],
            version: process.env.VUE_APP_VERSION
        };
    },
    methods: {
        readableTime(isoTime) {
            const date = new Date(isoTime);
            const dateTimeFormat = new Intl.DateTimeFormat('en-GB', {
                year: 'numeric',
                month: 'numeric',
                day: 'numeric',
                hour: "numeric",
                minute: "numeric",
                second: "numeric"
            });

            const retTime = dateTimeFormat.format(date)
            console.log(retTime);
            return retTime;
        },
        onResize() {
            this.windowWidth = window.innerWidth
        }
    },
    async mounted() {
        this.$nextTick(() => {
            window.addEventListener('resize', this.onResize);
        })

        this.isAdmin = await fetch(`${this.$root.$apiUrl}/api/admin`).then(response => response.status === 200);

        console.log(this.apiUrl);

        console.log(`Copyright chemistrying 2024. Version ${this.version}.`)
    }
}
</script>