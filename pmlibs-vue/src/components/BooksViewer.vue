<template>
    <!-- <div class="row row-cols-1 g-1">
        <div class="col" v-for="bookInput in bookshelf.allBooks.toReversed()">
            <div class="row row-cols-2">
                <div class="col-10">
                    <book-card :book="bookInput"></book-card>
                </div>
                <div class="col-2">
                    <button type="submit" class="d-flex h-100 align-items-center btn btn-danger" :class="activeClass" :disabled="bookshelf.status == 3">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                            <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5m3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0z"/>
                            <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4zM2.5 3h11V2h-11z"/>
                        </svg>
                    </button>
                </div>
            </div>
        </div>
    </div> -->
    <div class="accordion accordian-flush" id="accordionBooks">
        <div class="accordion-item" v-for="bookInput in bookshelf.allBooks.toReversed()">
            <book-card :book="bookInput" :bookshelf="bookshelf" :session="session"></book-card>
        </div>
    </div>
</template>

<script>
import BookCard from './BookCard.vue'

export default {
    props: ['bookshelf', 'session'],
    components: {
        BookCard
    },
    data() {
        
    },
    mounted() {
        console.log(this.bookshelf.allBooks);
    }
}
</script>