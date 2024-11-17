<template>
    <div class="row g-4">
        <div class="col-lg-3">
            <div class="card bg-primary-subtle border border-primary-subtle" style="auto">
                <div class="card-body">
                    <h4 class="card-title"> {{ session.sessionName }} </h4>
                    <p class="card-subtitle text-body-secondary"> #{{ session.id }} </p>
                    <p class="card-subtitle text-body-secondary mt-1"> {{ session.description }} </p>
                    <p class="card-text text-body-secondary">
                        Start Time: {{ this.$root.readableTime(session.startDate) }}
                    </p>
                    <p class="card-text"> Total Progress: </p>
                    <div class="progress" role="progressbar" aria-label="Basic example 20px high"
                        :aira-valuenow=progress aria-valuemin="0" aria-valuemax="100">
                        <div class="progress-bar" role="progressbar" :style="`width: ${progress}%`"></div>
                    </div>
                    <div class="row g-1 mt-1">
                        <export-button></export-button>
                        <return-button></return-button>
                    </div>
                </div>
            </div>

            <!-- manual margin????? -->
            <div class="card mt-2 bg-light border border-light-subtle" style="auto" v-if="!initLoading">
                <div class="card-body">
                    <h4 class="card-title"> {{ bookshelf.groupName + '-' + bookshelf.shelfNumber }} </h4>
                    <p class="card-subtitle text-body-secondary"> #{{ bookshelf.id }} </p>
                    <p class="card-subtitle text-body-secondary mt-1"> {{ bookshelf.description }} </p>
                    <p class="card-text text-body-secondary" v-if="bookshelf.status !== 0"> Start Time: {{
                        this.$root.readableTime(bookshelf.startTime) }} </p>
                    <button type="submit" class="btn btn-success" @click.prevent="startStocktake"
                        v-if="bookshelf.status === 0">
                        Start
                    </button>
                    <button type="submit" class="btn btn-success" @click.prevent="finishStocktake"
                        v-if="bookshelf.status === 1 || bookshelf.status === 2">
                        Finish
                    </button>
                </div>
            </div>
        </div>
        <div class="col row-cols-1">
            <book-input-form v-if="!initLoading" :session="session" :bookshelf="bookshelf"></book-input-form>
            <div class="alert alert-warning" role="alert" v-if="countBook">
                {{ countBookResponse.message }}
            </div>
            <books-viewer v-if="!loading" :bookshelf="bookshelf" :session="session"></books-viewer>
        </div>
    </div>
</template>

<script>
import BooksViewer from './BooksViewer.vue';
import BookInputForm from './BookInputForm.vue';
import ReturnButton from './ReturnButton.vue';
import ExportButton from './ExportButton.vue';

export default {
    props: ['session'],
    components: {
        BooksViewer, BookInputForm, ReturnButton, ExportButton
    },
    computed: {

    },
    data() {
        return {
            progress: 50,
            loading: true,
            initLoading: true,
            bookshlef: {},
            timer: null,
            countBook: false,
            countBookResponse: {}
        }
    },
    methods: {
        async refreshBookshelf() {
            this.loading = true;
            this.bookshelf = await fetch(`${this.$root.apiUrl}/api/bookshelves/${this.$root.bookshelfId}`).then(response => response.json());
            this.loading = false

            if (this.bookshelf.status != 2) {
                this.countBook = false;
            }
        },
        async startStocktake() {
            var data = {
                sessionId: this.session.id,
                bookshelfId: this.bookshelf.id,
                operation: this.$root.operations.START
            }

            console.log(data);

            var response = await fetch(`${this.$root.apiUrl}/api/stocktake`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            console.log(response.json());

            this.refreshBookshelf();
        },
        async finishStocktake() {
            var data = {
                sessionId: this.session.id,
                bookshelfId: this.bookshelf.id,
                operation: this.$root.operations.FINISH
            }

            console.log(data);

            var response = await fetch(`${this.$root.apiUrl}/api/stocktake`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            }).then(res => res.json());

            console.log(response);

            if (response.verdict == "warning") {
                this.countBook = true;
                this.countBookResponse = response;
            } else {
                this.$root.activePage = this.$root.appPage.GROUP
            }

            this.refreshBookshelf();

        }
    },
    async mounted() {
        // this.bookshelves = await this.session.allBookshelfGroups.filter(group => group.groupName == this.$root.bookshelfGroup)[0].allBookshelvesId;
        console.log("Mounting BookshelfViewer...")
        this.bookshelf = await fetch(`${this.$root.apiUrl}/api/bookshelves/${this.$root.bookshelfId}`).then(response => response.json());
        this.progress = await fetch(`${this.$root.apiUrl}/api/progress/${this.session.id}/${this.$root.bookshelfGroup}`).then(response => response.json());
        this.loading = false;
        this.initLoading = false;
        console.log(this.bookshelf);

        // console.log("Set a fetch timer... (fetch every 2s)")
        // this.timer = setInterval(async () => {
        //     this.bookshelf = await fetch(`${this.$root.apiUrl}/api/bookshelves/${this.$root.bookshelfId}`).then(response => response.json());
        // }, 2000);
    }
}
</script>