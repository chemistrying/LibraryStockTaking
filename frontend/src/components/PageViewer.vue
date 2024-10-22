<template>
    <div class="container">
        <div
            class="container-fluid text-center mt-5 mb-5 pt-4 pb-4 bg-light-subtle border border-light-subtle rounded-3">
            <div class="row">
                <h1> {{ page.pageTitle }} </h1>
            </div>
            <div class="row">
                <p> {{ page.content }} </p>
            </div>
        </div>
        <Transition>
            <session-list v-if="this.$root.activePage == this.$root.appPage.ROOT"></session-list>
        </Transition>
        <Transition>
            <session-viewer :session="retreiveCurrentSession()"
                v-if="this.$root.activePage == this.$root.appPage.SESSION"></session-viewer>
        </Transition>
        <Transition>
            <bookshelf-group-viewer :session="retreiveCurrentSession()"
                v-if="this.$root.activePage == this.$root.appPage.GROUP"></bookshelf-group-viewer>
        </Transition>
        <Transition>
            <bookshelf-viewer :session="retreiveCurrentSession()"
                v-if="this.$root.activePage == this.$root.appPage.BOOKSHELF"></bookshelf-viewer>
        </Transition>
    </div>
    <div class="container w-100 mb-3 mt-3 text-center text-secondary">
        © chemistrying, 2024. Version {{ this.$root.version }}.
        <div class="container">
            <a href="https://github.com/chemistrying/libraryStockTaking">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
                    class="bi bi-github github-svg" viewBox="0 0 16 16">
                    <path
                        d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27s1.36.09 2 .27c1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.01 8.01 0 0 0 16 8c0-4.42-3.58-8-8-8" />
                </svg>
            </a>
        </div>
    </div>
</template>

<script>
import SessionList from './SessionList.vue'
import SessionViewer from './SessionViewer.vue'
import BookshelfGroupViewer from './BookshelfGroupViewer.vue'
import BookshelfViewer from './BookshelfViewer.vue'

export default {
    // props: camelCase!
    components: {
        SessionList, SessionViewer, BookshelfGroupViewer, BookshelfViewer
    },
    props: ['page'],
    methods: {
        retreiveCurrentSession() {
            return this.$root.allSessionsInfo.filter(session => session.id === this.$root.sessionId)[0];
        }
    }
}
</script>

<style>
/* we will explain what these classes do next! */
.v-enter-active {
    transition: all 1s ease;
}

.v-leave-active {
    transition: all 0s ease;
}

.v-enter-from {
    transform: translateY(20px);
    opacity: 0;
}

.v-leave-to {
    opacity: 0;
}

.github-svg {
    fill: black !important
}
</style>