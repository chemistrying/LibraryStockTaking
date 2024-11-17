<template>
    <a class="nav-link" :class="activeClasses" aria-current="page" :href="page.link.url"
        :title="`This link goes to the ${page.link.text.toLowerCase()} page`">
        {{ page.link.text }}
    </a>
</template>

<script>
export default {
    props: ['page', 'isActive', 'pageIndex'],
    computed: {
        activeClasses() {
            return {
                active: this.isActive,
                emphasize: this.isActive,
                disabled: this.pageIndex > Math.max(0, this.$root.activePage)
            }
        }
    }
}
</script>

<!-- scope -> scoping for CSS -->
<style>
.emphasize {
    text-decoration: underline !important;
}
</style>