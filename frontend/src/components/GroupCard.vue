<template>
    <div class="card h-100 w-100" style="auto">
        <div class="card-body">
            <h4 class="card-title"> {{ group.groupName }} </h4>
            <p class="card-text"> Number of bookshelves: {{ group.allBookshelvesId.length }} </p>
            <h5 class="card-text"> Progress: </h5>
            <div class="progress" role="progressbar" aria-label="Basic example 20px high" :aira-valuenow=progress
                aria-valuemin="0" aria-valuemax="100">
                <div class="progress-bar" role="progressbar" :style="`width: ${progress}%`"></div>
            </div>
            <button href="#" class="btn btn-primary mt-2" @click.prevent="jumpToGroup()"> Enter </button>
        </div>
    </div>
</template>

<script>
export default {
    props: ['session', 'group'],
    computed: {

    },
    data() {
        return {
            progress: 50
        }
    },
    methods: {
        jumpToGroup() {
            this.$root.activePage = this.$root.appPage.GROUP;
            this.$root.bookshelfGroup = this.group.groupName;
            console.log(this.$root.bookshelfGroup);
        }
    },
    async mounted() {
        this.progress = await fetch(`${this.$root.apiUrl}/api/progress/${this.session.id}/${this.group.groupName}`).then(response => response.json());
    }
}
</script>