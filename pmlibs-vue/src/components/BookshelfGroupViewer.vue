<template>
    <div class="row g-4">
        <div class="col-lg-3">
            <div class="card bg-primary-subtle border border-primary-subtle" style="auto">
                <div class="card-body">
                    <h4 class="card-title"> {{ session.sessionName }} </h4>
                    <p class="card-subtitle text-body-secondary"> #{{ session.id }} </p>
                    <p class="card-subtitle text-body-secondary mt-1"> {{ session.description }} </p>
                    <p class="card-text text-body-secondary"> Start Time: {{ this.$root.readableTime(session.startDate) }} </p>
                    <p class="card-text"> Total Progress: </p>
                    <div class="progress" role="progressbar" aria-label="Basic example 20px high" :aira-valuenow=progress aria-valuemin="0" aria-valuemax="100">
                        <div class="progress-bar" role="progressbar" :style="`width: ${progress}%`"></div>
                    </div>
                    <div class="row g-1 mt-1">
                        <export-button></export-button>
                        <return-button></return-button>
                    </div>
                </div>
            </div>
        </div>
        <div class="col">
            <div class="row row-cols-1 g-4">
                <div class="col" v-for="bookshelf in bookshelves.filter(bookshelf => bookshelf.status != 3)">
                    <bookshelf-card :session="session" :bookshelf="bookshelf"></bookshelf-card>
                </div>
                <div class="col" v-for="bookshelf in bookshelves.filter(bookshelf => bookshelf.status == 3)">
                    <bookshelf-card :session="session" :bookshelf="bookshelf"></bookshelf-card>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import BookshelfCard from './BookshelfCard.vue';
import ReturnButton from './ReturnButton.vue';
import ExportButton from './ExportButton.vue';

export default {
    props: ['session'],
    components: {
        BookshelfCard, ReturnButton, ExportButton
    },
    computed: {

    },
    data() {
        return {
            progress: 50,
            bookshelves: []         
        }
    },
    methods: {

    },
    async mounted() {
        // this.bookshelves = await this.session.allBookshelfGroups.filter(group => group.groupName == this.$root.bookshelfGroup)[0].allBookshelvesId;
        this.bookshelves = await fetch(`${this.$root.apiUrl}/api/bookshelves/${this.session.id}/${this.$root.bookshelfGroup}`).then(response => response.json());
        this.progress = await fetch(`${this.$root.apiUrl}/api/progress/${this.session.id}/${this.$root.bookshelfGroup}`).then(response => response.json());
        console.log(this.bookshelves);
    }
}
</script>