<template>
    <div class="card h-100 w-100" :class="cardClass" style="auto">
        <div class="card-body">
            <h4 class="card-text"> {{ bookshelf.groupName + '-' + bookshelf.shelfNumber }} </h4>
            <p class="card-subtitle text-body-secondary"> #{{ bookshelf.id }} </p>
            <p class="card-subtitle text-body-secondary"> {{ bookshelf.description }} </p>
            <p class="card-text"> 
                Status: 
                <span class="badge rounded-pill bg-danger" v-if="bookshelf.status == 0"> Not yet started </span>
                <span class="badge rounded-pill bg-warning" v-if="bookshelf.status == 1 || bookshelf.status == 2"> In progress </span>
                <span class="badge rounded-pill bg-success" v-if="bookshelf.status == 3"> Finished </span>
            </p>
            <button href="#" class="btn mt-2" :class="buttonClass" @click.prevent="jumpToBookshelf()"> {{ this.enterButtonText() }} </button>
        </div>
    </div>
</template>


<script>
export default {
    props: ['session', 'bookshelf'],
    computed: {
        cardClass() {
            return {
                'bg-success-subtle': this.bookshelf.status == 3,
                'bg-warning-subtle': this.bookshelf.status == 1 || this.bookshelf.status == 2,
                'bg-danger-subtle': this.bookshelf.status == 0
            }
        },
        buttonClass() {
            return {
                'btn-success': this.bookshelf.status == 3,
                'btn-warning': this.bookshelf.status == 1 || this.bookshelf.status == 2,
                'btn-danger': this.bookshelf.status == 0
            }
        }
    },
    methods: {
        enterButtonText() {
            if (this.bookshelf.status == 0) {
                return "Start";
            } else if (this.bookshelf.status == 3) {
                return "View";
            }
            return "Enter";
        },
        jumpToBookshelf() {
            this.$root.activePage = this.$root.appPage.BOOKSHELF;
            this.$root.bookshelfId = this.bookshelf.id
            console.log("HELLO " + this.$root.bookshelfId);
        }
    }
}
</script>